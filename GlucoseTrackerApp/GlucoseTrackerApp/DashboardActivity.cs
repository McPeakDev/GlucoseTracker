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
    public class DashboardActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string token = Intent.GetStringExtra("token");
           
            SetContentView(Resource.Layout.activity_dashboard);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_dashboard);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_dashboard);
            navigationView.SetNavigationItemSelectedListener(this);

            AppCompatButton logoutButton = FindViewById<AppCompatButton>(Resource.Id.logout_button);
            AppCompatButton addExerciseButton = FindViewById<AppCompatButton>(Resource.Id.exercise_add_button);

            logoutButton.Click += delegate
            {
                OnLogoutPressed();
            };

            addExerciseButton.Click += delegate
            {
                OnAddExercisePressed(token);
            };
        }

        public void OnAddExercisePressed(string token)
        {
            Intent exerciseActivity = new Intent(this, typeof(ExerciseAddActivity));
            exerciseActivity.PutExtra("token", token);
            StartActivity(exerciseActivity);
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
                // Handle the camera action
            }
            else if (id == Resource.Id.nav_food)
            {

            }
            else if (id == Resource.Id.nav_bloodsugar)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
    }
}