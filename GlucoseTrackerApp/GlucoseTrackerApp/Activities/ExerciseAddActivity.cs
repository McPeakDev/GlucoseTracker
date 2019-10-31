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

namespace GlucoseTrackerApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Design.NoActionBar")]
    public class ExerciseAddActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppCompatEditText Hours { get;  set; }
        private AppCompatEditText LevelBefore { get; set; }
        private AppCompatEditText LevelAfter { get; set; }
        private AppCompatEditText MealName { get; set; }

        private string _token;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_exercise_add);

            _token = Intent.GetStringExtra("token");

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
            Hours = FindViewById<AppCompatEditText>(Resource.Id.exercise_hours);


            addExerciseButton.Click += delegate
            {
                OnAddExercisePressed(_token);
            };

        }

        public async void OnAddExercisePressed(string token)
        {
            DateTime timeNow = DateTime.Now;
            RestService restAPI = new RestService(token);

            PatientData patientData = new PatientData();
            Patient patient = await restAPI.ReadPatient();

            PatientExercise patientExercise = new PatientExercise()
            { 
               UserId = patient.UserId,
                HoursExercised = float.Parse(Hours.Text),
                TimeOfDay = timeNow
            };

            patientData.PatientExercises.Add(patientExercise);

            restAPI.CreatePatientData(patientData);

            FinishAfterTransition();
        }



        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_dashboard)
            {
                Intent dashboardActivity = new Intent(this, typeof(DashboardActivity));
                dashboardActivity.PutExtra("token", _token);
                StartActivity(dashboardActivity);
            }
            else if (id == Resource.Id.nav_exercise)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseAddActivity));
                exerciseActivity.PutExtra("token", _token);
                StartActivity(exerciseActivity);
            }
            else if (id == Resource.Id.nav_food)
            {
                Intent queryFoodActivity = new Intent(this, typeof(QueryFoodActivity));
                queryFoodActivity.PutExtra("token", _token);
                StartActivity(queryFoodActivity);
            }
            else if (id == Resource.Id.nav_bloodsugar)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarAddActivity));
                bloodSugarActivity.PutExtra("token", _token);
                StartActivity(bloodSugarActivity);
            }
            else if (id == Resource.Id.nav_logout)
            {
                Intent loginActivity = new Intent(this, typeof(LoginActivity));
                StartActivity(loginActivity);
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            FinishAfterTransition();
            return true;
        }
    }

}