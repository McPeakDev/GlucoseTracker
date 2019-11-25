///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         ExerciseModifyActivity.cs
//	Description:       
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly RestService _restService = RestService.GetRestService();
        private AppCompatSpinner _entries;
        private AppCompatEditText _hours;
        private AppCompatEditText _middleField;
        private AppCompatEditText _bottomField;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_modify);

            _entries = FindViewById<AppCompatSpinner>(Resource.Id.spinner);
            _hours = FindViewById<AppCompatEditText>(Resource.Id.top_field);
            _middleField = FindViewById<AppCompatEditText>(Resource.Id.middle_field);
            _bottomField = FindViewById<AppCompatEditText>(Resource.Id.bottom_field);

            _hours.InputType = Android.Text.InputTypes.NumberFlagDecimal;

            _middleField.Visibility = ViewStates.Gone;
            _bottomField.Visibility = ViewStates.Gone;

            AppCompatButton bloodSugarEditButton = FindViewById<AppCompatButton>(Resource.Id.submit_button);
            AppCompatButton bloodSugarDeleteButton = FindViewById<AppCompatButton>(Resource.Id.delete_button);


            _hours.Hint = "Hours Exercised";

            _hours.Visibility = ViewStates.Gone;


            bloodSugarEditButton.Click += async delegate
            {
                string status = await OnExerciseEditButtonPressed();
                if (status == "Success")
                {
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                }
            };

            bloodSugarDeleteButton.Click += async delegate
            {
                string status = await Task.Run(() => OnExerciseDeleteButtonPressed());
                if (status == "Success")
                {
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                }
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_modify);
            toolbar.Title = "Modify An Exercise";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            _entries.ItemSelected += delegate
            {
                _hours.Visibility = ViewStates.Visible;

                PatientExercise pe = _entries.SelectedItem.Cast<PatientExercise>();

                _hours.Text = pe.HoursExercised.ToString();
            };

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_modify);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected async override void OnStart()
        {
            base.OnStart();

            PatientData patientData = await _restService.ReadPatientDataAsync();

            if (!(patientData is null))
            {
                ArrayAdapter<PatientExercise> adapter = new ArrayAdapter<PatientExercise>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, patientData.PatientExercises.Where(pe => pe.TimeOfDay.ToLocalTime().Date == DateTime.Now.ToLocalTime().Date).OrderBy(pe => pe.TimeOfDay.ToLocalTime()).ToList());
                _entries.Adapter = adapter;
            }
            else
            {
                Toast.MakeText(this, "No Connection", ToastLength.Long).Show();
                Intent loginActivity = new Intent(this, typeof(LoginActivity));
                StartActivity(loginActivity);
                Finish();
            }
        }

        public async Task<string> OnExerciseEditButtonPressed()
        {
            if (!String.IsNullOrEmpty(_hours.Text))
            {
                if (float.Parse(_hours.Text) > 0 && float.Parse(_hours.Text) <= 4)
                {

                    PatientData patientData = new PatientData();
                    Patient patient = await _restService.ReadPatientAsync();

                    PatientExercise patientExercise = new PatientExercise()
                    {
                        ExerciseId = _entries.SelectedItem.Cast<PatientExercise>().ExerciseId,
                        UserId = patient.UserId,
                        HoursExercised = float.Parse(_hours.Text),
                        TimeOfDay = _entries.SelectedItem.Cast<PatientExercise>().TimeOfDay
                    };

                    patientData.PatientExercises.Add(patientExercise);

                    try
                    {
                        _restService.UpdatePatientDataAsync(patientData);
                    }
                    catch (Exception)
                    {
                        Intent loginActivity = new Intent(this, typeof(LoginActivity));
                        StartActivity(loginActivity);
                        Finish();
                    }
                    return "Success";
                }
                else
                {
                    return "That Number is Invalid"; ;
                }
            }
            else
            {
                return "Form Is Not Filled Out";
            }
        }

        public string OnExerciseDeleteButtonPressed()
        {
            if (!(_entries.SelectedItem is null))
            {
                PatientData patientData = new PatientData();

                patientData.PatientExercises.Add(_entries.SelectedItem.Cast<PatientExercise>());

                _restService.DeletePatientDataAsync(patientData);

                return "Success";
            }
            else
            {
                return "Invalid Selection";
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
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarAddActivity));
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_bloodsugar_modify)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarModifyActivity));
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_carbs)
            {
                Intent carbActivity = new Intent(this, typeof(CarbAddActivity));
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