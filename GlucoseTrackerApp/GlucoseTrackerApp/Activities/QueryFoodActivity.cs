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
    public class QueryFoodActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppCompatEditText Name { get;  set; }
        private string _token;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_query_food);

            _token = Intent.GetStringExtra("token");

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_query_food);
            toolbar.Title = "Find a Food Item";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_query_food);
            navigationView.SetNavigationItemSelectedListener(this);

            AppCompatButton queryFoodButton = FindViewById<AppCompatButton>(Resource.Id.query_food_button);
            Name = FindViewById<AppCompatEditText>(Resource.Id.query_food_name);

            queryFoodButton.Click += delegate
            {
                OnQueryFoodPressed(_token);
            };

        }

        public async void OnQueryFoodPressed(string token)
        {
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