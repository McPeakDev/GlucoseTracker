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
    [Activity(Label = "Add A Blood Sugar Reading", Theme = "@style/Theme.Design.NoActionBar")]
    public class BloodSugarAddActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AppCompatEditText Level;
        private AppCompatEditText MealName;
        private AppCompatSpinner ReadingType;
        private string _token;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_blood_sugar_add);

            _token = Intent.GetStringExtra("token");

            Level = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_reading);
            MealName = FindViewById<AppCompatEditText>(Resource.Id.blood_sugar_add_meal_name);
            ReadingType = FindViewById<AppCompatSpinner>(Resource.Id.blood_sugar_reading_type);

            ArrayAdapter<ReadingType> adapter = new ArrayAdapter<ReadingType>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, (Enum.GetValues(typeof(ReadingType)).Cast<ReadingType>().ToArray()));
            ReadingType.Adapter = adapter;

            AppCompatButton bloodSugarAddButton = FindViewById<AppCompatButton>(Resource.Id.blood_sugar_add_button);

            bloodSugarAddButton.Click += async delegate
            {
                string status = await OnBloodSugarAddButtonPressed();
                if (status == "Success")
                {
                    Finish();
                }
                else if (status == "No Connection")
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

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }

        public async Task<string> OnBloodSugarAddButtonPressed()
        {
            if (!String.IsNullOrEmpty(Level.Text) && !String.IsNullOrEmpty(MealName.Text))
            {
                if ( float.Parse(Level.Text) <= 1000 && float.Parse(Level.Text) > 0 && !String.IsNullOrEmpty(MealName.Text))
                {
                    DateTime timeNow = DateTime.Now;
                    RestService restAPI = new RestService(_token);

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

                    PatientBloodSugar patientBlood = new PatientBloodSugar()
                    {
                        UserId = patient.UserId,
                        Level = float.Parse(Level.Text),
                       // ReadingType = ReadingType.SelectedItem.Cast<ReadingType>(),    
                        TimeOfDay = timeNow
                    };

                    MealItem mealItem;

                    try
                    {
                        mealItem = await restAPI.ReadMealItemAsync(MealName.Text);


                        if (!(mealItem is null))
                        {
                            patientBlood.MealId = mealItem.MealId;
                        }
                        else
                        {
                            int fdcId = await restAPI.FindMealDataAsync(MealName.Text);
                            int carbs = (int)await restAPI.ReadMealDataAsync(fdcId);

                            var words = MealName.Text.Split(" ");

                            string mealName;

                            for (int i = 0; i < words.Length; i++)
                            {
                                string word = words[i];

                                words[i] = (word.Substring(0, 1).ToUpper() + word.Substring(1, word.Length - 1).ToLower());
                            }

                            mealName = String.Join(" ", words);

                            mealItem = new MealItem()
                            {
                                Carbs = carbs,
                                FoodName = mealName
                            };

                            await restAPI.CreateMealItemAsync(mealItem);

                            mealItem = await restAPI.ReadMealItemAsync(MealName.Text);
                            patientBlood.MealId = mealItem.MealId;
                        }
                    }
                    catch (Exception)
                    {
                        return "Invalid Food Name";
                    }

                    PatientCarbohydrate patientCarbohydrate = new PatientCarbohydrate()
                    {
                        UserId = patient.UserId,
                        MealId = mealItem.MealId,
                        Patient = patient,
                        TimeOfDay = timeNow,
                        FoodCarbs = mealItem.Carbs
                    };

                    patientData.PatientCarbohydrates.Add(patientCarbohydrate);
                    patientData.PatientBloodSugars.Add(patientBlood);

                    restAPI.CreatePatientData(patientData);
                    
                    return "Success";
                }
                else
                {
                    return "Invalid Range";
                }
            }
            else
            {
                return "Form Is Not Filled Out";
            }
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