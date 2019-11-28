///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         LoginActivity.cs
//	Description:       Methods for users logging into the GlucoseTrackerApp
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using GlucoseAPI.Models.Entities;
using GlucoseTrackerApp.Services;
using Android.Widget;
using Android.Content;
using System.Threading.Tasks;

namespace GlucoseTrackerApp
{ 
    [Activity(Label = "Glucose Tracker", Theme = "@style/Theme.MaterialComponents.Light.NoActionBar", MainLauncher = true)]
    public class LoginActivity : AppCompatActivity
    {
        private readonly RestService _restService = RestService.GetRestService();
        private string _token;
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


            loginButton.Click += async delegate
            {
                loginButton.Enabled = false;
                registerButton.Enabled = false;
                string status = await OnLoginPressedAsync(_email.Text, _password.Text);
                if (status == "Success")
                {
                    Intent dashboardActivity = new Intent(this, typeof(DashboardActivity));
                    StartActivity(dashboardActivity);
                    Finish();
                }
                else
                {
                    loginButton.Enabled = true;
                    registerButton.Enabled = true;
                    RunOnUiThread(() => 
                    {
                        Toast.MakeText(this, status, ToastLength.Long).Show(); 
                    });  
                }

            };

            registerButton.Click += async delegate
            {
                loginButton.Enabled = false;
                registerButton.Enabled = false;
                await Task.Run(OnRegisterPressed);
                loginButton.Enabled = true;
                registerButton.Enabled = true;
            };


            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.Title = "Login to Glucose Tracker";
            SetSupportActionBar(toolbar);

        }

        public override void OnBackPressed()
        {
            FinishAndRemoveTask();
        }

        public async Task<string> OnLoginPressedAsync(string email, string password)
        {

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return "Both Fields Are Required";
            }
            else
            {
                Credentials loginCreds = new Credentials()
                {
                    Email = email,
                    Password = password
                };

                _token = await _restService.LoginAsync(loginCreds);

                if (_token is null)
                {
                    _password.Text = String.Empty;
                    return "No Connection";
                }
                else if (_token != "Invalid Credentials" && _token != "")
                {
                    if (_autoEmail.Checked)
                    {
                        Storage.SaveEmail(email);
                    }
                    else
                    {
                        Storage.DeleteFile();
                    }
                    return "Success";
                }
                else
                {
                    _password.Text = String.Empty;
                    return "Email / Password Combination Was Invalid. Please Try Again";
                }

            }
        }

        public void OnRegisterPressed()
        {
            Intent registerActivity = new Intent(this, typeof(RegisterActivity));
            StartActivity(registerActivity);
        }
    }
}

