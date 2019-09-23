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
    [Activity(Label = "@string / app_name", Theme = "@style / AppTheme")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //get UI controls
            EditText username = FindViewById<EditText>(Resource.Id.usernameText);
            EditText password = FindViewById<EditText>(Resource.Id.passwordText);
            Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
            Button createButton = FindViewById<Button>(Resource.Id.createButton);


            loginButton.Click += (sender, e) =>
            {
                if (username.Text == null)
                {
                    //
                }
                else if (password.Text == null)
                {
                    //
                }
                else
                {
                    Patient user = Login.LoginPatient(username.Text, password.Text);
                    if(user != null)
                    {
                        //
                    }
                    else
                    {
                        password.Text = "";
                    }
                }
            };

            createButton.Click += (sender, e) =>
            {
                //
            };
        }

        
    }
}