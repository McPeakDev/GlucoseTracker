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
                if (!String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Password.Text) && !String.IsNullOrEmpty(FirstName.Text) && !String.IsNullOrEmpty(LastName.Text) && !String.IsNullOrEmpty(PhoneNumber.Text))
                {
                    RestService restAPI;

                    Regex emailRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");
                    Regex passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
                    Regex phoneRegex = new Regex(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$");

                    if(!emailRegex.IsMatch(Email.Text.Trim()))
                    {
                        throw new Exception("Must match the form of example@example.com");
                    }

                    if (!passwordRegex.IsMatch(Password.Text.Trim()))
                    {
                        throw new Exception("Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number");
                    }

                    if (PhoneNumber.Text.Length != 10)
                    {
                        if(!phoneRegex.IsMatch(PhoneNumber.Text.Trim()))
                        {
                            throw new Exception("Invalid Phone Number");
                        }
                        throw new Exception("Must be 10 digits long");
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

                    if (!String.IsNullOrEmpty(DoctorToken.Text))
                    {
                        restAPI = new RestService(DoctorToken.Text.Trim());
                        Doctor doctor = await restAPI.ReadDoctorAsync();

                        if (!(doctor is null))
                        {
                            patient.DoctorId = doctor.UserId;
                        }
                        else
                        {
                            throw new Exception("Invalid Doctor Token");
                        }
                    }

                    PatientCreationBundle patientCreationBundle = new PatientCreationBundle()
                    {
                        Password = Password.Text.Trim(),
                        Patient = patient
                    };

                    restAPI = new RestService();

                    string status = await restAPI.RegisterAsync(patientCreationBundle);

                    if (status != "Invalid User")
                    {
                        Toast.MakeText(this, "Registered!", ToastLength.Long).Show();

                        Finish();
                    }
                    else 
                    {
                        throw new Exception("User Already Exists");
                    }
                }
                else
                {
                    Toast.MakeText(this, "What Has Been Entered Is Invalid. Please Try Again.", ToastLength.Long).Show();
                    Password.Text = String.Empty;
                }
            }
            catch(Exception e)
            {
                Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                Password.Text = String.Empty;
            }
        }
    }
}