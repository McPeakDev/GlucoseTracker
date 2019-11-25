///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         CarbAddActivity.cs
//	Description:       Methods for creating new carbs for the carb graph
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
    public class CarbAddActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppCompatSpinner Carbs;
        private AppCompatEditText FoodCarbs;
        private AppCompatEditText MiddleField;
        private AppCompatEditText MealName;

        RestService _restAPI;
        private string _token;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_modify);

            _token = Intent.GetStringExtra("token");
            _restAPI = new RestService(_token);

            Carbs = FindViewById<AppCompatSpinner>(Resource.Id.spinner);
            FoodCarbs = FindViewById<AppCompatEditText>(Resource.Id.top_field);
            MiddleField = FindViewById<AppCompatEditText>(Resource.Id.middle_field);
            MealName = FindViewById<AppCompatEditText>(Resource.Id.bottom_field);

            Carbs.Visibility = ViewStates.Gone;
            MiddleField.Visibility = ViewStates.Gone;

            AppCompatButton carbCreateButton = FindViewById<AppCompatButton>(Resource.Id.submit_button);
            AppCompatButton carbDeleteButton = FindViewById<AppCompatButton>(Resource.Id.delete_button);


            FoodCarbs.Hint = "Food Carbs";
            MealName.Hint = "Meal Name";

            carbCreateButton.Text = "Add Carb Reading";
            carbDeleteButton.Visibility = ViewStates.Gone;

            carbCreateButton.Click += async delegate
            {
                string status = await OnCarbCreateButtonPressed();
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

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_modify);
            toolbar.Title = "Add A Carb Reading";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_modify);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }

        public async Task<string> OnCarbCreateButtonPressed()
        {
            if(!String.IsNullOrEmpty(FoodCarbs.Text))
            {
                if (int.Parse(FoodCarbs.Text) > 0 && int.Parse(FoodCarbs.Text) <= 1000)
                    {
                    DateTime timeNow = DateTime.Now.ToLocalTime();

                    PatientData patientData = new PatientData();
                    Patient patient;

                    try
                    {
                        patient = await _restAPI.ReadPatientAsync();
                    }
                    catch (Exception)
                    {
                        return "No Connection";
                    }

                    PatientCarbohydrate patientCarb = new PatientCarbohydrate()
                    {
                        UserId = patient.UserId,
                        FoodCarbs = int.Parse(FoodCarbs.Text),
                        TimeOfDay = timeNow
                    };


                    MealItem mealItem;

                    try
                    {
                        mealItem = await _restAPI.ReadMealItemAsync(MealName.Text);

                        if (!(mealItem is null))
                        {
                            patientCarb.MealId = mealItem.MealId;
                        }
                        else
                        {
                            int fdcId = await _restAPI.FindMealDataAsync(MealName.Text);
                            int carbs = (int)await _restAPI.ReadMealDataAsync(fdcId);

                            var words = MealName.Text.Split(" ");

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

                            await _restAPI.CreateMealItemAsync(mealItem);

                            mealItem = await _restAPI.ReadMealItemAsync(MealName.Text);
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
                        _restAPI.CreatePatientData(patientData);
                    }
                    catch(Exception)
                    {
                        return "No Connection";
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
                exerciseActivity.PutExtra("token", _token);
                StartActivity(exerciseActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_exercise_modify)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseModifyActivity));
                exerciseActivity.PutExtra("token", _token);
                StartActivity(exerciseActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_bloodsugar)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarAddActivity));
                bloodSugarActivity.PutExtra("token", _token);
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_bloodsugar_modify)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarModifyActivity));
                bloodSugarActivity.PutExtra("token", _token);
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_carbs)
            {
                Intent carbActivity = new Intent(this, typeof(CarbAddActivity));
                carbActivity.PutExtra("token", _token);
                StartActivity(carbActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_carbs_modify)
            {
                Intent carbActivity = new Intent(this, typeof(CarbModifyActivity));
                carbActivity.PutExtra("token", _token);
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
                alert.SetMessage(_token.Substring(_token.Length - 6, 6));
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