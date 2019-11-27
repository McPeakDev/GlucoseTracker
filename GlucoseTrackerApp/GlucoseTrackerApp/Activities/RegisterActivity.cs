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
    [Activity(Label = "Register Patient", Theme = "@style/Theme.MaterialComponents.Light.NoActionBar")]
    public class RegisterActivity : AppCompatActivity
    {
        private readonly RestService _restService = RestService.GetRestService();
        private AppCompatEditText _email;
        private AppCompatEditText _password;
        private AppCompatEditText _firstName;
        private AppCompatEditText _middleName;
        private AppCompatEditText _lastName;
        private AppCompatEditText _phoneNumber;
        private AppCompatEditText _doctorToken; 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_register);

            _email = FindViewById<AppCompatEditText>(Resource.Id.email);
            _password = FindViewById<AppCompatEditText>(Resource.Id.password);
            _firstName = FindViewById<AppCompatEditText>(Resource.Id.first_name);
            _middleName = FindViewById<AppCompatEditText>(Resource.Id.middle_name);
            _lastName = FindViewById<AppCompatEditText>(Resource.Id.last_name);
            _phoneNumber = FindViewById<AppCompatEditText>(Resource.Id.phone_number);
            _doctorToken = FindViewById<AppCompatEditText>(Resource.Id.doctor_token);

            AppCompatButton registerButton = FindViewById<AppCompatButton>(Resource.Id.register_button);

            registerButton.Click += async delegate
            {
                registerButton.Enabled = false;
                string status = await OnRegisterButtonPressedAsync();
                Toast.MakeText(this, status, ToastLength.Long).Show();
                if(status == "Registered!")
                {
                    Finish();
                }
                else
                {
                    registerButton.Enabled = true;
                }
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_register);
            toolbar.Title = "Register Patient";
            SetSupportActionBar(toolbar);
        }

        public async Task<string> OnRegisterButtonPressedAsync()
        {
            if (!String.IsNullOrEmpty(_email.Text) && !String.IsNullOrEmpty(_password.Text) && !String.IsNullOrEmpty(_firstName.Text) && !String.IsNullOrEmpty(_lastName.Text) && !String.IsNullOrEmpty(_phoneNumber.Text))
            {
                Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
                Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&]{8,}$");
                Regex phoneRegex = new Regex(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$");

                if (!emailRegex.IsMatch(_email.Text.Trim()))
                {
                    _password.Text = String.Empty;
                    return "Must match the form of example@example.com";

                }

                if (!passwordRegex.IsMatch(_password.Text.Trim()))
                {
                    _password.Text = String.Empty;
                    return "Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number";
                }

                if (_phoneNumber.Text.Length != 10)
                {
                    if (!phoneRegex.IsMatch(_phoneNumber.Text.Trim()))
                    {
                        _password.Text = String.Empty;
                        return "Invalid Phone Number";
                    }
                    _password.Text = String.Empty;
                    return "Phone Number must be 10 digits long";
                }

                Patient patient = new Patient()
                {
                    Email = _email.Text.Trim(),
                    FirstName = _firstName.Text.Trim(),
                    LastName = _lastName.Text.Trim(),
                    PhoneNumber = _phoneNumber.Text.Trim(),
                };

                if (!String.IsNullOrEmpty(_middleName.Text))
                {
                    patient.MiddleName = _middleName.Text.Trim();
                }

                PatientCreationBundle patientCreationBundle = new PatientCreationBundle()
                {
                    Password = _password.Text.Trim(),
                    Patient = patient
                };

                if (!String.IsNullOrEmpty(_doctorToken.Text))
                {
                    patientCreationBundle.DoctorToken = _doctorToken.Text.Trim();
                }

                string status = await _restService.RegisterAsync(patientCreationBundle);

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
                    _password.Text = String.Empty;
                    return "User Already Exists or Invalid Doctor Token";
                }
            }
            else
            {
                _password.Text = String.Empty;
                return "Form Is Not Filled";
            }
        }
    }
}