using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using GlucoseAPI.Models.Entities;
using GlucoseTrackerApp.Services;
using Android.Widget;
using Android.Content;

namespace GlucoseTrackerApp
{ 
    [Activity(Label = "Glucose Tracker", Theme = "@style/Theme.Design.NoActionBar", MainLauncher = true)]
    public class LoginActivity : AppCompatActivity
    {
        private AppCompatEditText Email { get; set; }
        private AppCompatEditText Password { get; set; }
        private AppCompatCheckBox AutoLogin { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Email = FindViewById<AppCompatEditText>(Resource.Id.email);
            Password = FindViewById<AppCompatEditText>(Resource.Id.password);

            AppCompatButton loginButton = FindViewById<AppCompatButton>(Resource.Id.login_button);
            AppCompatButton registerButton = FindViewById<AppCompatButton>(Resource.Id.register_button);

            loginButton.Click += delegate
            {
                OnLoginPressedAsync(Email.Text, Password.Text);
            };

            registerButton.Click += delegate
            {
                OnRegisterPressedAsync(Email.Text, Password.Text);
            };

            AutoLogin = FindViewById<AppCompatCheckBox>(Resource.Id.auto_login);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Login to Glucose Tracker";
            SetSupportActionBar(toolbar);

        }

        public override void OnBackPressed()
        {
            FinishAndRemoveTask();
        }

        public async void OnLoginPressedAsync(string email, string password)
        {

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                Toast.MakeText(this, "Both Fields Are Required", ToastLength.Long).Show();
            }
            else
            {
                RestService restAPI = new RestService();

                Credentials loginCreds = new Credentials()
                {
                    Email = email,
                    Password = password
                };

                string token = await restAPI.LoginAsync(loginCreds);

                if (token != "Invalid Credentials")
                {
                    Intent dashboardActivity = new Intent(this, typeof(DashboardActivity));
                    dashboardActivity.PutExtra("token", token);
                    StartActivity(dashboardActivity);
                    FinishAfterTransition();
                }
                else
                {
                    Password.Text = String.Empty;
                    Toast.MakeText(this, "Email / Password Combination Was Invalid. Please Try Again.", ToastLength.Long).Show();
                }

            }
        }

        public void OnRegisterPressedAsync(string email, string password)
        {
            Intent registerActivity = new Intent(this, typeof(RegisterActivity));
            StartActivity(registerActivity);
        }
    }
}

