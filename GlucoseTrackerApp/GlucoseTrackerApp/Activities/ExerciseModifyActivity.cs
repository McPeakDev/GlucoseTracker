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
    public class ExerciseModifyActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppCompatSpinner Entries;
        private AppCompatEditText Hours;
        private AppCompatEditText MiddleField;
        private AppCompatEditText BottomField;

        RestService _restAPI;
        private string _token;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_modify);

            _token = Intent.GetStringExtra("token");
            _restAPI = new RestService(_token);

            Entries = FindViewById<AppCompatSpinner>(Resource.Id.spinner);
            Hours = FindViewById<AppCompatEditText>(Resource.Id.top_field);
            MiddleField = FindViewById<AppCompatEditText>(Resource.Id.middle_field);
            BottomField = FindViewById<AppCompatEditText>(Resource.Id.bottom_field);

            Hours.InputType = Android.Text.InputTypes.NumberFlagDecimal;

            MiddleField.Visibility = ViewStates.Gone;
            BottomField.Visibility = ViewStates.Gone;

            AppCompatButton bloodSugarEditButton = FindViewById<AppCompatButton>(Resource.Id.submit_button);
            AppCompatButton bloodSugarDeleteButton = FindViewById<AppCompatButton>(Resource.Id.delete_button);


            Hours.Hint = "Hours Exercised";

            Hours.Visibility = ViewStates.Gone;


            bloodSugarEditButton.Click += delegate
            {
                OnExerciseEditButtonPressed();
            };

            bloodSugarDeleteButton.Click += delegate
            {
                OnExerciseDeleteButtonPressed();
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_modify);
            toolbar.Title = "Modify An Exercise";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            Entries.ItemSelected += delegate
            {
                Hours.Visibility = ViewStates.Visible;

                PatientExercise pe = Entries.SelectedItem.Cast<PatientExercise>();

                Hours.Text = pe.HoursExercised.ToString();
            };

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_modify);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected async override void OnStart()
        {
            base.OnStart();

            PatientData patientData = await _restAPI.ReadPatientDataAsync();

            ArrayAdapter<PatientExercise> adapter = new ArrayAdapter<PatientExercise>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, patientData.PatientExercises.Where(pe => pe.TimeOfDay.ToLocalTime().Date == DateTime.Now.ToLocalTime().Date).OrderBy(pe => pe.TimeOfDay.ToLocalTime()).ToList());

            Entries.SetAdapter(adapter);
        }

        public async void OnExerciseEditButtonPressed()
        {
            try
            {
                if (float.Parse(Hours.Text) > 0 && float.Parse(Hours.Text) <= 4)
                {
                    DateTime timeNow = DateTime.Now.ToLocalTime();

                    PatientData patientData = new PatientData();
                    Patient patient = await _restAPI.ReadPatientAsync();

                    PatientExercise patientExercise = new PatientExercise()
                    {
                        ExerciseId = Entries.SelectedItem.Cast<PatientExercise>().ExerciseId,
                        UserId = patient.UserId,
                        HoursExercised = float.Parse(Hours.Text),
                        TimeOfDay = Entries.SelectedItem.Cast<PatientExercise>().TimeOfDay
                    };

                    MealItem mealItem = await _restAPI.ReadMealItemAsync(BottomField.Text);

                    patientData.PatientExercises.Add(patientExercise);

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

        public void OnExerciseDeleteButtonPressed()
        {
            try
            {
                PatientData patientData = new PatientData();

                patientData.PatientExercises.Add(Entries.SelectedItem.Cast<PatientExercise>());

                _restAPI.DeletePatientDataAsync(patientData);

                Finish();
            }
            catch (Exception)
            {
                Toast.MakeText(this, "What Has Been Entered Is Invalid. Please Try Again.", ToastLength.Long).Show();
            }
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
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