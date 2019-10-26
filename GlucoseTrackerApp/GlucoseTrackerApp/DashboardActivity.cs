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
    [Activity( Theme = "@style/Theme.Design.NoActionBar")]
    public class DashboardActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private RestService _restApi;
        private string _token;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _token = Intent.GetStringExtra("token");

            _restApi = new RestService(_token);

            Patient patient = await _restApi.ReadPatient();

            SetContentView(Resource.Layout.activity_dashboard);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_dashboard);
            toolbar.Title = $"Welcome, {patient.LastName},{patient.FirstName}";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_dashboard);
            navigationView.SetNavigationItemSelectedListener(this);

            AppCompatButton logoutButton = FindViewById<AppCompatButton>(Resource.Id.logout_button);
            AppCompatButton addExerciseButton = FindViewById<AppCompatButton>(Resource.Id.exercise_add_button);
            AppCompatButton addFoodButton = FindViewById<AppCompatButton>(Resource.Id.add_food_button);


            logoutButton.Click += delegate
            {
                OnLogoutPressed();
            };

            addExerciseButton.Click += delegate
            {
                OnAddExercisePressed(token);
            };

            addFoodButton.Click += delegate
            {
                OnAddFoodPressed(token);
            };
        }

        public void OnAddExercisePressed(string token)
        {
            Intent exerciseActivity = new Intent(this, typeof(ExerciseAddActivity));
            exerciseActivity.PutExtra("token", token);
            StartActivity(exerciseActivity);
        }

        public void OnAddFoodPressed(string token)
        {
            Intent queryFoodActivity = new Intent(this, typeof(QueryFoodActivity));
            queryFoodActivity.PutExtra("token", token);
            StartActivity(queryFoodActivity);
        }

        public void OnLogoutPressed()
        {
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
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
            else if (id == Resource.Id.nav_food)
            {

            }
            else if (id == Resource.Id.nav_bloodsugar)
            {

            }
            else if (id == Resource.Id.nav_logout)
            {
                Intent loginActivity = new Intent(this, typeof(LoginActivity));
                StartActivity(loginActivity);
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}