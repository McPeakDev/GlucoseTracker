﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            MiddleField.Visibility = ViewStates.Gone;

            AppCompatButton carbEditButton = FindViewById<AppCompatButton>(Resource.Id.submit_button);
            AppCompatButton carbDeleteButton = FindViewById<AppCompatButton>(Resource.Id.delete_button);


            FoodCarbs.Hint = "Food Carbs";
            MealName.Hint = "Meal Name";

            FoodCarbs.Visibility = ViewStates.Gone;
            MealName.Visibility = ViewStates.Gone;

            carbEditButton.Click += delegate
            {
                OnCarbEditButtonPressed();
            };

            carbDeleteButton.Click += delegate
            {
                OnCarbDeleteButtonPressed();
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_modify);
            toolbar.Title = "Modify A Carb Measurement";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            Carbs.ItemSelected += delegate
            {
                FoodCarbs.Visibility = ViewStates.Visible;
                MealName.Visibility = ViewStates.Visible;

                PatientCarbohydrate pc = Carbs.SelectedItem.Cast<PatientCarbohydrate>();

                FoodCarbs.Text = pc.FoodCarbs.ToString();
                MealName.Text = pc.Meal.FoodName;

            };

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_modify);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected async override void OnStart()
        {
            base.OnStart();

            PatientData patientData = await _restAPI.ReadPatientDataAsync();

            ArrayAdapter<PatientCarbohydrate> adapter = new ArrayAdapter<PatientCarbohydrate>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, patientData.PatientCarbohydrates.Where(pc => pc.TimeOfDay.ToLocalTime().Date == DateTime.Now.ToLocalTime().Date).OrderBy(pc => pc.TimeOfDay.ToLocalTime()).ToList());

            Carbs.SetAdapter(adapter);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }

        public async void OnCarbEditButtonPressed()
        {
            try
            {
                if (int.Parse(FoodCarbs.Text) > 0 && int.Parse(FoodCarbs.Text) <= 1000)
                    {
                    DateTime timeNow = DateTime.Now.ToLocalTime();

                    PatientData patientData = new PatientData();
                    Patient patient = await _restAPI.ReadPatientAsync();

                    PatientCarbohydrate patientCarb = new PatientCarbohydrate()
                    {
                        CarbId = Carbs.SelectedItem.Cast<PatientCarbohydrate>().CarbId,
                        UserId = patient.UserId,
                        FoodCarbs = int.Parse(FoodCarbs.Text),
                        TimeOfDay = Carbs.SelectedItem.Cast<PatientCarbohydrate>().TimeOfDay
                    };

                    MealItem mealItem = await _restAPI.ReadMealItemAsync(MealName.Text);

                    if (!(mealItem is null))
                    {
                        patientCarb.MealId = mealItem.MealId;
                    }
                    else
                    {
                        int fdcId = await _restAPI.FindMealDataAsync(MealName.Text);
                        int carbs = (int)await _restAPI.ReadMealDataAsync(fdcId);

                        mealItem = new MealItem()
                        {
                            Carbs = carbs,
                            FoodName = (MealName.Text.Substring(0, 1).ToUpper() + MealName.Text.Substring(1, MealName.Text.Length - 1).ToLower())
                        };

                        await _restAPI.CreateMealItemAsync(mealItem);

                        mealItem = await _restAPI.ReadMealItemAsync(MealName.Text);
                        patientCarb.MealId = mealItem.MealId;
                    }

                    patientData.PatientCarbohydrates.Add(patientCarb);

                    _restAPI.UpdatePatientDataAsync(patientData);

                    Finish();
                }
                else 
                {
                    Toast.MakeText(this, "What Has Been Entered Is Invalid. Please Try Again.", ToastLength.Long).Show();
                }
            }
            catch (Exception)
            {
                Toast.MakeText(this, "What Has Been Entered Is Invalid. Please Try Again.", ToastLength.Long).Show();
            }
        }

        public void OnCarbDeleteButtonPressed()
        {
            try
            {
                PatientData patientData = new PatientData();

                patientData.PatientCarbohydrates.Add(Carbs.SelectedItem.Cast<PatientCarbohydrate>());

                _restAPI.DeletePatientDataAsync(patientData);

                Finish();
            }
            catch (Exception)
            {
                Toast.MakeText(this, "What Has Been Entered Is Invalid. Please Try Again.", ToastLength.Long).Show();
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_exercise)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseAddActivity));
                exerciseActivity.PutExtra("token", _token);
                StartActivity(exerciseActivity);
            }
            else if (id == Resource.Id.nav_exercise_modify)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseModifyActivity));
                exerciseActivity.PutExtra("token", _token);
                StartActivity(exerciseActivity);
            }
            else if (id == Resource.Id.nav_bloodsugar)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarAddActivity));
                bloodSugarActivity.PutExtra("token", _token);
                StartActivity(bloodSugarActivity);
            }
            else if (id == Resource.Id.nav_bloodsugar_modify)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarModifyActivity));
                bloodSugarActivity.PutExtra("token", _token);
                StartActivity(bloodSugarActivity);
            }
            else if (id == Resource.Id.nav_carbs)
            {
                Intent carbActivity = new Intent(this, typeof(CarbAddActivity));
                carbActivity.PutExtra("token", _token);
                StartActivity(carbActivity);
            }
            else if (id == Resource.Id.nav_carbs_modify)
            {
                Intent carbActivity = new Intent(this, typeof(CarbModifyActivity));
                carbActivity.PutExtra("token", _token);
                StartActivity(carbActivity);
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
                alert.SetMessage(_token);
                alert.SetPositiveButton("Ok", (c, ev) =>
                {
                    //Do nothing
                });

                alert.Show();
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            Finish();
            return true;
        }
    }
}