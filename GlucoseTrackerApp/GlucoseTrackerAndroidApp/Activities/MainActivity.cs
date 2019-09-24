using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GlucoseTrackerAndroidApp.Services;
using GlucoseTrackerWeb.Models;
using System;
using System.Net;

namespace GlucoseTrackerAndroidApp
{
    [Activity(Label = "Glucose Tracker", Theme = "@android:style/Theme.Material", MainLauncher = true)]
    public class MainActivity : Activity
    {
        EditText email;
        EditText password;
        Button loginButton;
        Button createButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.activity_main);
            //get UI controls
            email = FindViewById<EditText>(Resource.Id.emailText);
            password = FindViewById<EditText>(Resource.Id.passwordText);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            createButton = FindViewById<Button>(Resource.Id.createButton);

            loginButton.Click += LoginAttempt_Click;

        }

        public void LoginAttempt_Click(object sender, EventArgs e)
        {
            RestService api = new RestService();
            Boolean status = api.LoginAsync(new Credentials()
            {
                Email = email.Text,
                Password = password.Text
            }).Result;

            if(status)
            {
                email.Text = "Success";
            }
            else
            {
                email.Text = "Failure";
            }


        }
    }
}

