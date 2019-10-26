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
        private AppCompatEditText DoctorID { get; set; }

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
            DoctorID = FindViewById<AppCompatEditText>(Resource.Id.doctor_id);

            AppCompatButton registerButton = FindViewById<AppCompatButton>(Resource.Id.register_button);


            registerButton.Click += delegate
            {
                OnRegisterButtonPressed();
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_register);
            toolbar.Title = "Register Patient";
            SetSupportActionBar(toolbar);
        }

        public async void OnRegisterButtonPressed ()
        {
            try
            {
                Patient patient = new Patient()
                {
                    Email = Email.Text.Trim(),
                    FirstName = FirstName.Text.Trim(),
                    MiddleName = MiddleName.Text.Trim(),
                    LastName = LastName.Text.Trim(),
                    PhoneNumber = PhoneNumber.Text.Trim(),
                    DoctorId = int.Parse(DoctorID.Text.Trim())
                };

                PatientCreationBundle patientCreationBundle = new PatientCreationBundle()
                {
                    Password = Password.Text.Trim(),
                    Patient = patient
                };

                RestService restAPI = new RestService();

                restAPI.RegisterAsync(patientCreationBundle);

                Toast.MakeText(this, "Registered!", ToastLength.Long).Show();

                Finish();
            }
            catch (Exception)
            {
                Toast.MakeText(this, "What Has Been Entered Is Invalid. Please Try Again.", ToastLength.Long).Show();
            }
        }
    }
}