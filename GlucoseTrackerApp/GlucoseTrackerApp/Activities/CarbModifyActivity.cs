///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         CarbModifyActivity.cs
//	Description:       Methods to delete previous carb entries
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GlucoseAPI.Models.Entities;
using GlucoseTrackerApp.Services;

namespace GlucoseTrackerApp
{
    [Activity(Label = "BloodSugarModifyActivity")]
    public class CarbModifyActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private readonly RestService _restService = RestService.GetRestService();
        private AppCompatSpinner _carbs;
        private AppCompatEditText _foodCarbs;
        private AppCompatEditText _middleField;
        private AppCompatEditText _mealName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_modify);

            _carbs = FindViewById<AppCompatSpinner>(Resource.Id.spinner);
            _foodCarbs = FindViewById<AppCompatEditText>(Resource.Id.top_field);
            _middleField = FindViewById<AppCompatEditText>(Resource.Id.middle_field);
            _mealName = FindViewById<AppCompatEditText>(Resource.Id.bottom_field);

            _middleField.Visibility = ViewStates.Gone;

            AppCompatButton carbEditButton = FindViewById<AppCompatButton>(Resource.Id.submit_button);
            AppCompatButton carbDeleteButton = FindViewById<AppCompatButton>(Resource.Id.delete_button);


            _foodCarbs.Hint = "Food Carbs";
            _mealName.Hint = "Meal Name";

            _foodCarbs.Visibility = ViewStates.Gone;
            _mealName.Visibility = ViewStates.Gone;

            carbEditButton.Click += async delegate
            {
                string status = await OnCarbEditButtonPressed();
                if (status == "Success")
                {
                    Finish();
                }
                else if (status == "No Connection")
                {
                    Intent loginActivity = new Intent(this, typeof(LoginActivity));
                    StartActivity(loginActivity);
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                }
            };

            carbDeleteButton.Click += async delegate
            {
                string status = await Task.Run(() => OnCarbDeleteButtonPressed());
                if (status == "Success")
                {
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                }
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_modify);
            toolbar.Title = "Modify A Carb Measurement";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            _carbs.ItemSelected += delegate
            {
                _foodCarbs.Visibility = ViewStates.Visible;
                _mealName.Visibility = ViewStates.Visible;

                PatientCarbohydrate pc = _carbs.SelectedItem.Cast<PatientCarbohydrate>();

                _foodCarbs.Text = pc.FoodCarbs.ToString();
                _mealName.Text = pc.Meal.FoodName;

            };

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_modify);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected async override void OnStart()
        {
            base.OnStart();

            PatientData patientData = await _restService.ReadPatientDataAsync();

            if (!(patientData is null))
            {
                ArrayAdapter<PatientCarbohydrate> adapter = new ArrayAdapter<PatientCarbohydrate>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, patientData.PatientCarbohydrates.Where(pc => pc.TimeOfDay.ToLocalTime().Date == DateTime.Now.ToLocalTime().Date).OrderBy(pc => pc.TimeOfDay.ToLocalTime()).ToList());
                _carbs.Adapter = adapter;
            }
            else
            {
                Toast.MakeText(this, "No Connection", ToastLength.Long).Show();
                Intent loginActivity = new Intent(this, typeof(LoginActivity));
                StartActivity(loginActivity);
                Finish();
            }
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }

        public async Task<string> OnCarbEditButtonPressed()
        {
            if(!String.IsNullOrEmpty(_foodCarbs.Text))
            {
                if (int.Parse(_foodCarbs.Text) > 0 && int.Parse(_foodCarbs.Text) <= 1000)
                {
                    PatientData patientData = new PatientData();
                    Patient patient = await _restService.ReadPatientAsync();

                    PatientCarbohydrate patientCarb = new PatientCarbohydrate()
                    {
                        CarbId = _carbs.SelectedItem.Cast<PatientCarbohydrate>().CarbId,
                        UserId = patient.UserId,
                        FoodCarbs = int.Parse(_foodCarbs.Text),
                        TimeOfDay = _carbs.SelectedItem.Cast<PatientCarbohydrate>().TimeOfDay
                    };


                    MealItem mealItem;

                    try
                    {
                        mealItem = await _restService.ReadMealItemAsync(_mealName.Text);

                        if (!(mealItem is null))
                        {
                            patientCarb.MealId = mealItem.MealId;
                        }
                        else
                        {
                            int fdcId = await _restService.FindMealDataAsync(_mealName.Text);
                            int carbs = (int)await _restService.ReadMealDataAsync(fdcId);

                            var words = _mealName.Text.Split(" ");

                            string mealName;

                            for (int i = 0; i < words.Length; i++)
                            {
                                string word = words[i];

                                words[i] = (word.Substring(0, 1).ToUpper() + word.Substring(1, word.Length - 1).ToLower());
                            }

                            mealName = String.Join(" ", words);

                            mealItem = new MealItem()
                            {
                                Carbs = carbs,
                                FoodName = mealName
                            };

                            await _restService.CreateMealItemAsync(mealItem);

                            mealItem = await _restService.ReadMealItemAsync(_mealName.Text);
                            patientCarb.MealId = mealItem.MealId;
                        }
                    }
                    catch (Exception)
                    {
                        return "Invalid Food Name";
                    }

                    patientData.PatientCarbohydrates.Add(patientCarb);

                    try
                    {
                        _restService.UpdatePatientDataAsync(patientData);
                    }
                    catch (Exception)
                    {
                        Intent loginActivity = new Intent(this, typeof(LoginActivity));
                        StartActivity(loginActivity);
                        Finish();
                    }
                    return "Success";
                }
                else
                {
                    return "That Number Is Invalid";
                }
            }
            else
            {
                return "The Form Is Not Filled Out";
            }
        }

        public string OnCarbDeleteButtonPressed()
        {
            if (!(_carbs.SelectedItem is null))
            {
                PatientData patientData = new PatientData();

                patientData.PatientCarbohydrates.Add(_carbs.SelectedItem.Cast<PatientCarbohydrate>());

                _restService.DeletePatientDataAsync(patientData);

                return "Success";
            }
            else
            {
                return "Invalid Selection";
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_dashboard)
            {
                Finish();
            }
            else if (id == Resource.Id.nav_exercise)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseAddActivity));
                StartActivity(exerciseActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_exercise_modify)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseModifyActivity));
                StartActivity(exerciseActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_bloodsugar)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarAddActivity));
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_bloodsugar_modify)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarModifyActivity));
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_carbs)
            {
                Intent carbActivity = new Intent(this, typeof(CarbAddActivity));
                StartActivity(carbActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_carbs_modify)
            {
                Intent carbActivity = new Intent(this, typeof(CarbModifyActivity));
                StartActivity(carbActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_logout)
            {
                Intent loginActivity = new Intent(this, typeof(LoginActivity));
                StartActivity(loginActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_token)
            {
                var alert = new Android.App.AlertDialog.Builder(this);

                alert.SetTitle("Patient Token");
                alert.SetMessage(_restService.UserToken.Substring(_restService.UserToken.Length - 6, 6));
                alert.SetPositiveButton("Ok", (c, ev) =>
                {
                    //Do nothing
                });

                alert.Show();
                alert.Dispose();
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}