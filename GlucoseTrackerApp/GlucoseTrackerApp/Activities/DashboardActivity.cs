///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         DashboardActivity.cs
//	Description:       creates the dashboard interface for the mobile application. populates charts 
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
using Microcharts.Droid;
using Microcharts;
using System.Collections.Generic;
using SkiaSharp;
using Android.Graphics;
using System.Linq;
using System.Timers;

namespace GlucoseTrackerApp
{
    [Activity(Label = "Dashboard", Theme = "@style/Theme.MaterialComponents.Light.NoActionBar")]
    public class DashboardActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private readonly RestService _restService = RestService.GetRestService();
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private ChartView _bloodChart;
        private ChartView _exerciseChart;
        private ChartView _carbChart;
        private TextView _bloodLabel;
        private TextView _exerciseLabel;
        private TextView _carbLabel;
        private DateTime _stopTimeStamp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_dashboard);

            _bloodChart = FindViewById<ChartView>(Resource.Id.bloodChart);
            _exerciseChart = FindViewById<ChartView>(Resource.Id.exerciseChart);
            _carbChart = FindViewById<ChartView>(Resource.Id.carbChart);

            _bloodLabel = FindViewById<TextView>(Resource.Id.blood_sugar_label);
            _exerciseLabel = FindViewById<TextView>(Resource.Id.exercise_label);
            _carbLabel = FindViewById<TextView>(Resource.Id.carb_label);

            _toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_dashboard);
            SetSupportActionBar(_toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, _toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_dashboard);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected async override void OnResume()
        {
            base.OnResume();
            Patient patient = await _restService.ReadPatientAsync();

            if (!(patient is null))
            {
                _toolbar.Title = $"Welcome, {patient.FirstName} {patient.LastName}";

                PopulateCharts(patient);
            }
        }

        protected async override void OnRestart()
        {
            base.OnRestart();
            if (DateTime.Now > _stopTimeStamp.AddMinutes(5))
            {
                Intent loginActivity = new Intent(this, typeof(LoginActivity));
                _restService.UserToken = null;
                StartActivity(loginActivity);
                Finish();
            }
            else 
            { 
                Patient patient = await _restService.ReadPatientAsync();

                if (!(patient is null))
                {
                    _toolbar.Title = $"Welcome, {patient.FirstName} {patient.LastName}";

                    PopulateCharts(patient);
                }
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            _stopTimeStamp = DateTime.Now;
        }

        private string PopulateCharts(Patient patient)
        {
            var bloodEntries = new List<ChartEntry>();
            var exerciseEntries = new List<ChartEntry>();
            var carbEntries = new List<ChartEntry>();

            patient.PatientBloodSugars = patient.PatientBloodSugars.OrderBy(bs => bs.TimeOfDay.ToLocalTime()).Where(bs => bs.TimeOfDay.ToLocalTime().Date <= DateTime.Today && bs.TimeOfDay.ToLocalTime().Date >= DateTime.Today.AddDays(-2)).ToList();
            patient.PatientCarbs = patient.PatientCarbs.OrderBy(pc => pc.TimeOfDay.ToLocalTime()).Where(pc => pc.TimeOfDay.ToLocalTime().Date <= DateTime.Today && pc.TimeOfDay.ToLocalTime().Date >= DateTime.Today.AddDays(-2)).ToList();
            patient.PatientExercises = patient.PatientExercises.OrderBy(pe => pe.TimeOfDay.ToLocalTime()).Where(pe => pe.TimeOfDay.ToLocalTime().Date <= DateTime.Today && pe.TimeOfDay.ToLocalTime().Date >= DateTime.Today.AddDays(-2)).ToList();

            foreach (var bloodSugar in patient.PatientBloodSugars)
            {
                if (bloodSugar.Meal != null)
                {
                    bloodEntries.Add(new ChartEntry(bloodSugar.Level)
                    {
                        Label = bloodSugar.TimeOfDay.ToLocalTime().DayOfWeek + $", {bloodSugar.TimeOfDay.ToLocalTime().Day} " + bloodSugar.ReadingType.ToString() + " " + bloodSugar.Meal.MealTime,
                        ValueLabel = (bloodSugar.Level).ToString(),
                        Color = SKColors.Maroon
                    });
                }
                else
                {
                    bloodEntries.Add(new ChartEntry(bloodSugar.Level)
                    {
                        Label = bloodSugar.TimeOfDay.ToLocalTime().DayOfWeek + $", {bloodSugar.TimeOfDay.ToLocalTime().Day} " + $"{bloodSugar.ReadingType.ToString()} Meal",
                        ValueLabel = (bloodSugar.Level).ToString(),
                        Color = SKColors.Maroon
                    });
                }
            }

            foreach (var exercise in patient.PatientExercises)
            {
                exerciseEntries.Add(new ChartEntry(exercise.HoursExercised)
                {
                    Label = $"{exercise.TimeOfDay.ToLocalTime().DayOfWeek} {exercise.TimeOfDay.ToLocalTime().Day}, {exercise.TimeOfDay.ToLocalTime().ToShortTimeString()}",
                    ValueLabel = exercise.HoursExercised.ToString(),
                    Color = SKColors.DarkGreen
                });
            }

            foreach (var carb in patient.PatientCarbs)
            {
                carbEntries.Add(new ChartEntry(carb.FoodCarbs)
                {
                    Label = $"{carb.Meal.MealTime.ToString()}: {carb.TimeOfDay.ToLocalTime().DayOfWeek} "+ $", {carb.TimeOfDay.ToLocalTime().Day} ",
                    ValueLabel = carb.FoodCarbs.ToString(),
                    Color = SKColors.Blue
                });
            }

            if (bloodEntries.Count > 0)
            {
                var bloodChart = new LineChart()
                {
                    Entries = bloodEntries,
                    BackgroundColor = SKColors.Transparent,
                    LabelTextSize = 30,
                    LabelOrientation = Microcharts.Orientation.Horizontal,
                    ValueLabelOrientation = Microcharts.Orientation.Horizontal,

                };
                _bloodChart.Chart = bloodChart;
                _bloodLabel.Text = "Blood Sugars";
                _bloodChart.Visibility = ViewStates.Visible;

            }
            else
            {
                _bloodLabel.Text = "No Blood Sugar Data";
                _bloodChart.Visibility = ViewStates.Gone;
            }

            if (exerciseEntries.Count > 0)
            {
                var exerciseChart = new LineChart()
                {
                    Entries = exerciseEntries,
                    BackgroundColor = SKColors.Transparent,
                    LabelTextSize = 30,
                    LabelOrientation = Microcharts.Orientation.Horizontal,
                    ValueLabelOrientation = Microcharts.Orientation.Horizontal

                };
                _exerciseChart.Chart = exerciseChart;
                _exerciseLabel.Text = "Exercise Hours";
                _exerciseChart.Visibility = ViewStates.Visible;

            }
            else
            {
                _exerciseLabel.Text = "No Exercise Data";
                _exerciseChart.Visibility = ViewStates.Gone;
            }

            if (carbEntries.Count > 0)
            {
                var carbChart = new LineChart()
                {
                    Entries = carbEntries,
                    BackgroundColor = SKColors.Transparent,
                    LabelTextSize = 30,
                    LabelOrientation = Microcharts.Orientation.Horizontal,
                    ValueLabelOrientation = Microcharts.Orientation.Horizontal
                };
                _carbChart.Chart = carbChart;
                _carbLabel.Text = "Carb Intake";
                _carbChart.Visibility = ViewStates.Visible;

            }
            else
            {
                _carbLabel.Text = "No Carb Data";
                _carbChart.Visibility = ViewStates.Gone;
            }


            return "Success";
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_exercise)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseAddActivity));
                StartActivity(exerciseActivity);
            }
            else if (id == Resource.Id.nav_exercise_modify)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseModifyActivity));
                StartActivity(exerciseActivity);
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
                });

                alert.SetNegativeButton("No", (c, ev) =>
                {
                    alert.Dispose();

                    Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarAddActivity));
                    StartActivity(bloodSugarActivity);
                });

                alert.Show();
            }
            else if (id == Resource.Id.nav_bloodsugar_modify)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarModifyActivity));
                StartActivity(bloodSugarActivity);
            }
            else if (id == Resource.Id.nav_carbs)
            {
                Intent carbActivity = new Intent(this, typeof(QueryFoodActivity));
                StartActivity(carbActivity);
            }
            else if (id == Resource.Id.nav_carbs_modify)
            {
                Intent carbActivity = new Intent(this, typeof(CarbModifyActivity));
                StartActivity(carbActivity);
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