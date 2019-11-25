///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         RegisterActivity.cs
//	Description:       Methods for users to create a new account with GlucoseTracker
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
using System.Text.RegularExpressions;

namespace GlucoseTrackerApp
{
    [Activity(Label = "Register Patient", Theme = "@style/Theme.Design.NoActionBar")]
    public class RegisterActivity : AppCompatActivity
    {
        private AppCompatEditText Email { get; set; }
        private AppCompatEditText Password { get; set; }
        private AppCompatEditText FirstName { get; set; }
        private AppCompatEditText MiddleName { get; set; }
        private AppCompatEditText LastName { get; set; }
        private AppCompatEditText PhoneNumber { get; set; }
        private AppCompatEditText DoctorToken { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_register);

            Email = FindViewById<AppCompatEditText>(Resource.Id.email);
            Password = FindViewById<AppCompatEditText>(Resource.Id.password);
            FirstName = FindViewById<AppCompatEditText>(Resource.Id.first_name);
            MiddleName = FindViewById<AppCompatEditText>(Resource.Id.middle_name);
            LastName = FindViewById<AppCompatEditText>(Resource.Id.last_name);
            PhoneNumber = FindViewById<AppCompatEditText>(Resource.Id.phone_number);
            DoctorToken = FindViewById<AppCompatEditText>(Resource.Id.doctor_token);

            AppCompatButton registerButton = FindViewById<AppCompatButton>(Resource.Id.register_button);

            registerButton.Click += async delegate
            {
                string status = await OnRegisterButtonPressedAsync();
                Toast.MakeText(this, status, ToastLength.Long).Show();
                if(status == "Registered!")
                {
                    Finish();
                }
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_register);
            toolbar.Title = "Register Patient";
            SetSupportActionBar(toolbar);
        }

        public async Task<string> OnRegisterButtonPressedAsync()
        {
            if (!String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Password.Text) && !String.IsNullOrEmpty(FirstName.Text) && !String.IsNullOrEmpty(LastName.Text) && !String.IsNullOrEmpty(PhoneNumber.Text))
            {
                RestService restAPI = new RestService();

                Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
                Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&]{8,}$");
                Regex phoneRegex = new Regex(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$");

                if (!emailRegex.IsMatch(Email.Text.Trim()))
                {
                    Password.Text = String.Empty;
                    return "Must match the form of example@example.com";

                }

                if (!passwordRegex.IsMatch(Password.Text.Trim()))
                {
                    Password.Text = String.Empty;
                    return "Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number";
                }

                if (PhoneNumber.Text.Length != 10)
                {
                    if (!phoneRegex.IsMatch(PhoneNumber.Text.Trim()))
                    {
                        Password.Text = String.Empty;
                        return "Invalid Phone Number";
                    }
                    Password.Text = String.Empty;
                    return "Phone Number must be 10 digits long";
                }

                Patient patient = new Patient()
                {
                    Email = Email.Text.Trim(),
                    FirstName = FirstName.Text.Trim(),
                    LastName = LastName.Text.Trim(),
                    PhoneNumber = PhoneNumber.Text.Trim(),
                };

                if (!String.IsNullOrEmpty(MiddleName.Text))
                {
                    patient.MiddleName = MiddleName.Text.Trim();
                }

                PatientCreationBundle patientCreationBundle = new PatientCreationBundle()
                {
                    Password = Password.Text.Trim(),
                    Patient = patient
                };

                if (!String.IsNullOrEmpty(DoctorToken.Text))
                {
                    patientCreationBundle.DoctorToken = DoctorToken.Text.Trim();
                }

                string status = await restAPI.RegisterAsync(patientCreationBundle);

                if (status is null)
                {
                    return "No Connection";
                }
                else if (status != "Invalid User" && !(status is null) )
                {
                    return "Registered!";
                }
                else
                {
                    Password.Text = String.Empty;
                    return "User Already Exists or Invalid Doctor Token";
                }
            }
            else
            {
                Password.Text = String.Empty;
                return "Form Is Not Filled";
            }
        }
    }
}