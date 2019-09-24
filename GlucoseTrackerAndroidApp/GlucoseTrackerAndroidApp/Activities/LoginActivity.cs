using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GlucoseTrackerAndroidApp
{
    [Activity(Label = "Glucose Tracker", Theme = "@style/AppTheme", MainLauncher = true)]
    public class LoginActivity : Activity
    {
        EditText username;
        EditText password;
        Button loginButton;
        Button createButton;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login_page);
            // Create your application here
            //get UI controls
            username = FindViewById<EditText>(Resource.Id.usernameText);
            password = FindViewById<EditText>(Resource.Id.passwordText);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            createButton = FindViewById<Button>(Resource.Id.createButton);


            //loginButton.SetOnClickListener(View.IOnClickListener);

            //this.loginButton.SetOnClickListener(LoginAttempt);


            loginButton.Click += (sender, e) =>
            {
                
            };
        }

        public void LoginAttempt(View target)
        {
            createButton.Text = "hello";
                if (username.Text == null)
                {
                    //
                }
                else if (password.Text == null)
                {
                    //
                }
                else if (password.Text != null && username.Text != null)
                {
                    Patient user = Login.LoginPatient(username.Text, password.Text);
                    if (user != null)
                    {
                        //
                    }
                    else
                    {
                        username.Text = "landed";
                    }
                }
            
        }
    }
}