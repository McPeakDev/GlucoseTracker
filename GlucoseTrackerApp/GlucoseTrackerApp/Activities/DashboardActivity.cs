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
using Android.Graphics;

namespace GlucoseTrackerApp
{
    [Activity(Label = "Dashboard", Theme = "@style/Theme.Design.NoActionBar")]
    public class DashboardActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private RestService _restAPI;
        private string _token;
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private ChartView _bloodChart;
        private ChartView _exerciseChart;
        private ChartView _carbChart;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_dashboard);

            _bloodChart = FindViewById<ChartView>(Resource.Id.bloodChart);
            _exerciseChart = FindViewById<ChartView>(Resource.Id.exerciseChart);
            _carbChart = FindViewById<ChartView>(Resource.Id.carbChart);

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

            var bloodEntries = new List<ChartEntry>();
            var exerciseEntries = new List<ChartEntry>();
            var carbEntries = new List<ChartEntry>();

            foreach (var bloodSugar in patient.PatientBloodSugars)
            {
                bloodEntries.Add(new ChartEntry((bloodSugar.LevelBefore + bloodSugar.LevelAfter) /2)
                {
                    Label = bloodSugar.TimeOfDay.ToShortDateString(),
                    ValueLabel = ((bloodSugar.LevelBefore + bloodSugar.LevelAfter) / 2).ToString(),
                    Color = SKColors.Maroon
                });
            }

            foreach (var exercise in patient.PatientExercises)
            {
                exerciseEntries.Add(new ChartEntry(exercise.HoursExercised)
                {
                    Label = exercise.TimeOfDay.ToShortDateString(),
                    ValueLabel = exercise.HoursExercised.ToString(),
                    Color = SKColors.DarkGreen
                });
            }

            foreach (var carb in patient.PatientCarbs)
            {
                carbEntries.Add(new ChartEntry(carb.FoodCarbs)
                {
                    Label = carb.TimeOfDay.ToShortDateString(),
                    ValueLabel = carb.FoodCarbs.ToString(),
                    Color = SKColors.Yellow
                });
            }

            var bloodChart = new LineChart() 
            {
                Entries = bloodEntries,
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = 25,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                ValueLabelOrientation = Microcharts.Orientation.Horizontal

            };

            var exerciseChart = new LineChart()
            {
                Entries = exerciseEntries,
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = 25,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                ValueLabelOrientation = Microcharts.Orientation.Horizontal

            };
            
            var carbChart = new LineChart()
            {
                Entries = carbEntries,
                BackgroundColor = SKColors.Transparent,
                LabelTextSize = 25,
                LabelOrientation = Microcharts.Orientation.Horizontal,
                ValueLabelOrientation = Microcharts.Orientation.Horizontal
            };

            _bloodChart.Chart = bloodChart;
            _exerciseChart.Chart = exerciseChart;
            _carbChart.Chart = carbChart;

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