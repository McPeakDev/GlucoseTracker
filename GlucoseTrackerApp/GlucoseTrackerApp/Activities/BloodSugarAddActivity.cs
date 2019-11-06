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
    [Activity(Label = "Add A Blood Sugar Reading", Theme = "@style/Theme.Design.NoActionBar")]
    public class BloodSugarAddActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppCompatEditText LevelBefore { get; set; }
        private AppCompatEditText LevelAfter { get; set; }
        private AppCompatEditText MealName { get; set; }

        private string _token;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_blood_sugar_add);

            _token = Intent.GetStringExtra("token");

            LevelBefore= FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_before_reading);
            LevelAfter = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_after_reading);
            MealName = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_meal_name);

            AppCompatButton bloodSugarAddButton = FindViewById<AppCompatButton>(Resource.Id.blood_sugar_add_button);

            bloodSugarAddButton.Click += delegate
            {
                OnBloodSugarAddButtonPressed();
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

        public async void OnBloodSugarAddButtonPressed()
        {
            try
            {
                DateTime timeNow = DateTime.Now;
                RestService restAPI = new RestService(_token);

                PatientData patientData = new PatientData();
                Patient patient = await restAPI.ReadPatientAsync();

                PatientBloodSugar patientBlood = new PatientBloodSugar()
                {
                    UserId = patient.UserId,
                    LevelBefore = float.Parse(LevelBefore.Text),
                    LevelAfter = float.Parse(LevelAfter.Text),
                    //Meal =
                    TimeOfDay = timeNow
                };

                patientData.PatientBloodSugars.Add(patientBlood);

                restAPI.CreatePatientData(patientData);

                Intent dashboardActivity = new Intent(this, typeof(DashboardActivity));
                dashboardActivity.PutExtra("token", _token);
                StartActivity(dashboardActivity);

                FinishAfterTransition();
            }
            catch (Exception)
            {
                Toast.MakeText(this, "What Has Been Entered Is Invalid. Please Try Again.", ToastLength.Long).Show();
            }
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