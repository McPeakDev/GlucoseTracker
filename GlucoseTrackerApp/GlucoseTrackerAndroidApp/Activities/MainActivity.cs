using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;

namespace GlucoseTrackerAndroidApp
{
    [Activity(Label = "Glucose Tracker", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
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

            loginButton.Click += LoginAttempt_Click;

        }

        public void LoginAttempt_Click(object sender, EventArgs e)
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

