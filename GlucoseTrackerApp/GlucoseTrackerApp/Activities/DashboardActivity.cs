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
using Microcharts.Droid;
using Microcharts;
using System.Collections.Generic;
using SkiaSharp;

namespace GlucoseTrackerApp
{
    [Activity(Label = "Dashboard", Theme = "@style/Theme.Design.NoActionBar")]
    public class DashboardActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private RestService _restAPI;
        private string _token;
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private ChartView _bloodChart;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_dashboard);

            _bloodChart = FindViewById<ChartView>(Resource.Id.bloodChart);

            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_dashboard);
            SetSupportActionBar(_toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, _toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_dashboard);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected async override void OnStart()
        {
            base.OnStart();

            _token = Intent.GetStringExtra("token");

            _restAPI = new RestService(_token);

            Patient patient = await _restAPI.ReadPatient();

            _toolbar.Title = $"Welcome, {patient.LastName}, {patient.FirstName}";

            var entries = new List<ChartEntry>();

            foreach (var bloodSugar in patient.PatientBloodSugars)
            {
                entries.Add(new ChartEntry(bloodSugar.LevelBefore)
                {
                    Label = bloodSugar.TimeOfDay.ToShortDateString(),
                    ValueLabel = bloodSugar.LevelBefore.ToString(),
                    Color = SKColor.Parse("#800020")
                });
            }

            var chart = new LineChart() { Entries = entries };

            _bloodChart.Chart = chart;
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