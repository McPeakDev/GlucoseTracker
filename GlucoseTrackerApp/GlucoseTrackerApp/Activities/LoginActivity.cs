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
    [Activity(Label = "Glucose Tracker", Theme = "@style/Theme.Design.NoActionBar", MainLauncher = true)]
    public class LoginActivity : AppCompatActivity
    {
        private string Token;
        private AppCompatEditText Email;
        private AppCompatEditText Password;
        private AppCompatCheckBox AutoEmail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Email = FindViewById<AppCompatEditText>(Resource.Id.email);
            Password = FindViewById<AppCompatEditText>(Resource.Id.password);
            AutoEmail = FindViewById<AppCompatCheckBox>(Resource.Id.auto_email);

            string email = Storage.ReadEmail();

            if (!(email is null))
            {
                Email.Text = email;
                AutoEmail.Checked = true;
            }

            AppCompatButton loginButton = FindViewById<AppCompatButton>(Resource.Id.login_button);
            AppCompatButton registerButton = FindViewById<AppCompatButton>(Resource.Id.register_button);

            loginButton.Click += async delegate
            {
                string status = await OnLoginPressedAsync(Email.Text, Password.Text);
                if (status == "Success")
                {
                    Intent dashboardActivity = new Intent(this, typeof(DashboardActivity));
                    dashboardActivity.PutExtra("token", Token);
                    StartActivity(dashboardActivity);
                    Finish();
                }
                else
                {
                    Toast.MakeText(this, status, ToastLength.Long).Show();
                }

            };

            registerButton.Click += async delegate
            {
                await Task.Run(OnRegisterPressed);
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
                RestService restAPI = new RestService();

                Credentials loginCreds = new Credentials()
                {
                    Email = email,
                    Password = password
                };

                Token = await restAPI.LoginAsync(loginCreds);

                if (Token != "Invalid Credentials" && !(Token is null))
                {
                    if (AutoEmail.Checked)
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
                    Password.Text = String.Empty;
                    return "Email / Password Combination Was Invalid. Please Try Again.";
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

