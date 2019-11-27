///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         ExerciseAddActivity.cs
//	Description:       Methods for adding hours to exercise times 
//	Author:            Zachery Johnson, johnsonzd@etsu.edu
//  Copyright:         Zachery Johnson, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using static BCrypt.Net.BCrypt;
using GlucoseAPI.Models.Entities;
using GlucoseTrackerApp.Services;
using Android.Widget;
using Android.Content;
using System.Threading.Tasks;

namespace GlucoseTrackerApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.MaterialComponents.Light.NoActionBar")]
    public class ExerciseAddActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private readonly RestService _restService = RestService.GetRestService();
        private AppCompatEditText _hours;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_exercise_add);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_exercise_add);
            toolbar.Title = "Add a New Exercise Activity";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_exercise_add);
            navigationView.SetNavigationItemSelectedListener(this);

            AppCompatButton addExerciseButton = FindViewById<AppCompatButton>(Resource.Id.add_exercise_button);
            _hours = FindViewById<AppCompatEditText>(Resource.Id.exercise_hours);


            addExerciseButton.Click += async delegate
            {
                addExerciseButton.Enabled = false;
                string status = await OnAddExercisePressed();
                if (status == "Success")
                {
                    Finish();
                }
                else
                {
                    addExerciseButton.Enabled = true;
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                }
            };

        }

        public async Task<string> OnAddExercisePressed()
        {
            if (!String.IsNullOrEmpty(_hours.Text))
            {
                if (float.Parse(_hours.Text) > 0 && float.Parse(_hours.Text) <= 4)
                {
                    DateTime timeNow = DateTime.Now.ToLocalTime();

                    PatientData patientData = new PatientData();
                    Patient patient;

                    try
                    {
                        patient = await _restService.ReadPatientAsync();
                    }
                    catch (Exception)
                    {
                        return "No Connection";
                    }

                    PatientExercise patientExercise = new PatientExercise()
                    {
                        UserId = patient.UserId,
                        HoursExercised = float.Parse(_hours.Text),
                        TimeOfDay = timeNow
                    };

                    patientData.PatientExercises.Add(patientExercise);

                    _restService.CreatePatientData(patientData);
                    
                    return "Success";
                }
                else
                {
                    return "That Number Is Not Valid";
                }
            }
            else
            {
                return "The form is not filled out";
            }

        }

        protected override void OnRestart()
        {
            base.OnRestart();
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            _restService.UserToken = null;
            StartActivity(loginActivity);
            Finish();
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
                var alert = new Android.App.AlertDialog.Builder(this);

                alert.SetTitle("Alert");
                alert.SetMessage("Do you want to add a food item with this?");
                alert.SetPositiveButton("Yes", (c, ev) =>
                {
                    alert.Dispose();

                    Intent queryActivity = new Intent(this, typeof(QueryFoodActivity));
                    queryActivity.PutExtra("BloodSugar", true);
                    StartActivity(queryActivity);
                    Finish();
                });

                alert.SetNegativeButton("No", (c, ev) =>
                {
                    alert.Dispose();

                    Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarAddActivity));
                    StartActivity(bloodSugarActivity);
                    Finish();
                });

                alert.Show();
            }
            else if (id == Resource.Id.nav_bloodsugar_modify)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarModifyActivity));
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_carbs)
            {
                Intent carbActivity = new Intent(this, typeof(QueryFoodActivity));
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
                _restService.UserToken = null;
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