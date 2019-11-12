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
        private AppCompatEditText _email;
        private AppCompatEditText _password;
        private AppCompatCheckBox _autoEmail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            _email = FindViewById<AppCompatEditText>(Resource.Id.email);
            _password = FindViewById<AppCompatEditText>(Resource.Id.password);
            _autoEmail = FindViewById<AppCompatCheckBox>(Resource.Id.auto_email);

            string email = Storage.ReadEmail();

            if (!(email is null))
            {
                _email.Text = email;
                _autoEmail.Checked = true;
            }

            AppCompatButton loginButton = FindViewById<AppCompatButton>(Resource.Id.login_button);
            AppCompatButton registerButton = FindViewById<AppCompatButton>(Resource.Id.register_button);

            loginButton.Click += delegate
            {
                OnLoginPressedAsync(_email.Text, _password.Text);
            };

            registerButton.Click += delegate
            {
                OnRegisterPressedAsync(_email.Text, _password.Text);
            };


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
                    if (_autoEmail.Checked)
                    {
                        Storage.SaveEmail(email);
                    }
                    else
                    {
                        Storage.DeleteFile();
                    }

                    Intent dashboardActivity = new Intent(this, typeof(DashboardActivity));
                    dashboardActivity.PutExtra("token", token);
                    StartActivity(dashboardActivity);
                    FinishAfterTransition();
                }
                else
                {
                    _password.Text = String.Empty;
                    Toast.MakeText(this, "_email / _password Combination Was Invalid. Please Try Again.", ToastLength.Long).Show();
                }

            }
        }

        public void OnRegisterPressedAsync(string _email, string _password)
        {
            Intent registerActivity = new Intent(this, typeof(RegisterActivity));
            StartActivity(registerActivity);
        }
    }
}

