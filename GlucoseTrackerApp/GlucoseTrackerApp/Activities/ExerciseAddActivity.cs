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
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.Design.NoActionBar")]
    public class ExerciseAddActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppCompatEditText Hours { get;  set; }

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


            addExerciseButton.Click += async delegate
            {
                string status = await OnAddExercisePressed(_token);
                if (status == "Success")
                {
                    Finish();
                }
                else if(status == "No Connection")
                {
                    Intent loginActivity = new Intent(this, typeof(LoginActivity));
                    StartActivity(loginActivity);
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                }
            };

        }

        public async Task<string> OnAddExercisePressed(string token)
        {
            if (!String.IsNullOrEmpty(Hours.Text))
            {
                if (float.Parse(Hours.Text) > 0 && float.Parse(Hours.Text) <= 4)
                {
                    DateTime timeNow = DateTime.Now.ToLocalTime();
                    RestService restAPI = new RestService(token);

                    PatientData patientData = new PatientData();
                    Patient patient;

                    try
                    {
                        patient = await restAPI.ReadPatientAsync();
                    }
                    catch (Exception)
                    {
                        return "No Connection";
                    }

                    PatientExercise patientExercise = new PatientExercise()
                    {
                        UserId = patient.UserId,
                        HoursExercised = float.Parse(Hours.Text),
                        TimeOfDay = timeNow
                    };

                    patientData.PatientExercises.Add(patientExercise);

                    restAPI.CreatePatientData(patientData);
                    
                    return "Success";
                }
                else
                {
                    return "That Number Is Not Valid";
                }
            }
            else
            {
                return "The form is not filled out";
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
                exerciseActivity.PutExtra("token", _token);
                StartActivity(exerciseActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_exercise_modify)
            {
                Intent exerciseActivity = new Intent(this, typeof(ExerciseModifyActivity));
                exerciseActivity.PutExtra("token", _token);
                StartActivity(exerciseActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_bloodsugar)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarAddActivity));
                bloodSugarActivity.PutExtra("token", _token);
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_bloodsugar_modify)
            {
                Intent bloodSugarActivity = new Intent(this, typeof(BloodSugarModifyActivity));
                bloodSugarActivity.PutExtra("token", _token);
                StartActivity(bloodSugarActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_carbs)
            {
                Intent carbActivity = new Intent(this, typeof(CarbAddActivity));
                carbActivity.PutExtra("token", _token);
                StartActivity(carbActivity);
                Finish();
            }
            else if (id == Resource.Id.nav_carbs_modify)
            {
                Intent carbActivity = new Intent(this, typeof(CarbModifyActivity));
                carbActivity.PutExtra("token", _token);
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
                alert.SetMessage(_token.Substring(_token.Length - 6, 6));
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