///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         BloodSugarAddActivity.cs
//	Description:       Methods for adding blood sugar readings for the mobile app
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using GlucoseAPI.Models.Entities;
using GlucoseTrackerApp.Services;
using Android.Widget;
using Android.Content;
using System.Threading.Tasks;
using System.Linq;

namespace GlucoseTrackerApp
{
    [Activity(Label = "Add A Blood Sugar Reading", Theme = "@style/Theme.MaterialComponents.Light.NoActionBar")]
    public class BloodSugarAddActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private readonly RestService _restService = RestService.GetRestService();
        private AppCompatEditText _level;
        private AppCompatEditText _mealName;
        private AppCompatSpinner _readingType;
        private AppCompatSpinner _mealType;
        private AppCompatEditText _carbs;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_blood_sugar_add);

            _level = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_reading);
            _mealName = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_meal_name);
            _readingType = FindViewById<AppCompatSpinner>(Resource.Id.blood_sugar_reading_type);
            _mealType = FindViewById<AppCompatSpinner>(Resource.Id.meal_type);
            _carbs = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_carbs);

            _carbs.Visibility = ViewStates.Gone;
            _mealName.Visibility = ViewStates.Gone;
            _mealType.Visibility = ViewStates.Gone;

            ArrayAdapter<ReadingType> adapter = new ArrayAdapter<ReadingType>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, (ReadingType[])Enum.GetValues(typeof(ReadingType)));
            _readingType.Adapter = adapter;

            AppCompatButton bloodSugarAddButton = FindViewById<AppCompatButton>(Resource.Id.blood_sugar_add_button);

            bloodSugarAddButton.Click += async delegate
            {
                bloodSugarAddButton.Enabled = false;

                string status = await OnBloodSugarAddButtonPressed();
                if (status == "Success")
                {
                    Finish();
                }
                else
                {
                    bloodSugarAddButton.Enabled = true;
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                }
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_blood_sugar_add);
            toolbar.Title = "Add A Blood Sugar Reading";
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view_blood_sugar_add);
            navigationView.SetNavigationItemSelectedListener(this);
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            _restService.UserToken = null;
            StartActivity(loginActivity);
            Finish();
        }

        public async Task<string> OnBloodSugarAddButtonPressed()
        {
            #region old code
            if (float.Parse(_level.Text) <= 1000 && float.Parse(_level.Text) > 0)
            {
                DateTime timeNow = DateTime.Now;

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

                PatientBloodSugar patientBlood = new PatientBloodSugar()
                {
                    UserId = patient.UserId,
                    ReadingType = (ReadingType)Enum.Parse(typeof(ReadingType), _readingType.SelectedItem.ToString()),
                    Level = float.Parse(_level.Text),
                    TimeOfDay = timeNow
                };

                patientData.PatientBloodSugars.Add(patientBlood);

                _restService.CreatePatientData(patientData);

                return "Success";
            }
            else
            {
                return "Invalid Range";
            }
            #endregion
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