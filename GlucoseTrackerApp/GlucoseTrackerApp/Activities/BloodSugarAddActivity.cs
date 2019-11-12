﻿using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using GlucoseAPI.Models.Entities;
using GlucoseTrackerApp.Services;
using Android.Widget;
using Android.Content;

namespace GlucoseTrackerApp
{
    [Activity(Label = "Add A Blood Sugar Reading", Theme = "@style/Theme.Design.NoActionBar")]
    public class BloodSugarAddActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppCompatEditText LevelBefore;
        private AppCompatEditText LevelAfter;
        private AppCompatEditText MealName;
        private string _token;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_blood_sugar_add);

            _token = Intent.GetStringExtra("token");

            LevelBefore= FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_before_reading);
            LevelAfter = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_after_reading);
            MealName = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_meal_name);

            AppCompatButton bloodSugarAddButton = FindViewById<AppCompatButton>(Resource.Id.blood_sugar_add_button);

            bloodSugarAddButton.Click += delegate
            {
                OnBloodSugarAddButtonPressed();
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_blood_sugar_add);
            toolbar.Title = "Add A Blood Sugar Reading";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_blood_sugar_add);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }

        public async void OnBloodSugarAddButtonPressed()
        {
            try
            {
                if (float.Parse(LevelBefore.Text) <= 1000 && float.Parse(LevelAfter.Text) <= 1000 && float.Parse(LevelBefore.Text) > 0 && float.Parse(LevelAfter.Text) > 0)
                {
                    DateTime timeNow = DateTime.Now.ToLocalTime();
                    RestService restAPI = new RestService(_token);

                    PatientData patientData = new PatientData();
                    Patient patient = await restAPI.ReadPatientAsync();

                    PatientBloodSugar patientBlood = new PatientBloodSugar()
                    {
                        UserId = patient.UserId,
                        LevelBefore = float.Parse(LevelBefore.Text),
                        LevelAfter = float.Parse(LevelAfter.Text),
                        TimeOfDay = timeNow
                    };

                    MealItem mealItem = await restAPI.ReadMealItemAsync(MealName.Text);

                    if (!(mealItem is null))
                    {
                        patientBlood.MealId = mealItem.MealId;
                    }
                    else
                    {
                        int fdcId = await restAPI.FindMealDataAsync(MealName.Text);
                        int carbs = (int)await restAPI.ReadMealDataAsync(fdcId);

                        mealItem = new MealItem()
                        {
                            Carbs = carbs,
                            FoodName = (MealName.Text.Substring(0, 1).ToUpper() + MealName.Text.Substring(1, MealName.Text.Length - 1).ToLower())
                        };

                        await restAPI.CreateMealItemAsync(mealItem);

                        mealItem = await restAPI.ReadMealItemAsync(MealName.Text);
                        patientBlood.MealId = mealItem.MealId;
                    }

                    PatientCarbohydrate patientCarbohydrate = new PatientCarbohydrate()
                    {
                        UserId = patient.UserId,
                        MealId = mealItem.MealId,
                        Patient = patient,
                        TimeOfDay = timeNow,
                        FoodCarbs = mealItem.Carbs
                    };

                    patientData.PatientCarbohydrates.Add(patientCarbohydrate);

                    patientData.PatientBloodSugars.Add(patientBlood);

                    restAPI.CreatePatientData(patientData);

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