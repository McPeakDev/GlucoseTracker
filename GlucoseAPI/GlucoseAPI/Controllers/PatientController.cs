///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  UserController.cs
//	Description:       API Interface for CRUD operations on Users
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Microsoft.AspNetCore.Mvc;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using static BCrypt.Net.BCrypt;

namespace GlucoseAPI.Controllers
{
    /// <summary>
    /// API Interface for CRUD operations on Users
    /// </summary>
    [Route("API/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Dependency Injection
        private readonly IRepository<Doctor> _doctorRepo;
        private readonly IRepository<Patient> _patientRepo;
        private readonly IRepository<PatientBloodSugar> _bloodSugarRepo;
        private readonly IRepository<PatientExercise> _exerciseRepo;
        private readonly IRepository<PatientCarbohydrates> _carbRepo;
        private readonly IRepository<Auth> _authRepo;
        private readonly IRepository<TokenAuth> _tokenAuthRepo;

        public UserController(IRepository<Doctor> doctorRepo, IRepository<Patient> patientRepo, IRepository<Auth> authRepo, IRepository<TokenAuth> tokenAuthRepo, IRepository<PatientBloodSugar> bloodSugarRepo, IRepository<PatientExercise> exerciseRepo, IRepository<PatientCarbohydrates> carbRepo)
        {
            _doctorRepo = doctorRepo;
            _patientRepo = patientRepo;
            _authRepo = authRepo;
            _tokenAuthRepo = tokenAuthRepo;
            _bloodSugarRepo = bloodSugarRepo;
            _exerciseRepo = exerciseRepo;
            _carbRepo = carbRepo;
        }
        #endregion

        #region CRUD
        /// <summary>
        /// Reads a User from the Database.
        /// </summary>
        /// <param name="token">A Users Identity</param>
        /// <returns>The Patient</returns>
        [HttpPost("Read")]
        public ActionResult<User> GrabUser([FromHeader(Name = "token")]string token)
        {
            //Grab the User's Id
            int? userId = _tokenAuthRepo.Read(a => a.Token == token).UserId;

            if (!(userId is null))
            {
                //Try to find the patient
                User user = _patientRepo.Read(p => p.UserId == userId.Value, p => p.PatientBloodSugars, p => p.PatientCarbs, p => p.PatientExercises, p => p.Doctor);

                //If the patient was found then return the patient
                if (!(user is null))
                {
                    foreach (var carb in (user as Patient).PatientCarbs)
                    {
                        carb.Meal = _carbRepo.Read(c => c.CarbId == carb.CarbId, c => c.Meal).Meal;
                    }
                    return user;
                }
                //Otherwise.. the user is a doctor
                else
                {
                    user = _doctorRepo.Read(d => d.UserId == userId.Value);
                    if (user is null)
                    {
                        return Content("Invalid Token");
                    }
                    return user;

                }
            }
            else
            {
                return Content("Invalid Token");
            }
        }

        /// <summary>
        /// Updates the Patient in the database
        /// </summary>
        /// <param name="token">A User's Identity</param>
        /// <param name="patient">An Updated Patient</param>
        /// <returns>A Respone Code</returns>
        [HttpPut("Update")]
        public IActionResult PutPatient([FromHeader(Name = "token")]string token, Patient patient)
        {
            try
            {
                //Verify the patient exists
                TokenAuth auth = _tokenAuthRepo.Read(t => t.Token == token);

                if (!(auth is null))
                {
                    //Update the Patient.
                    _patientRepo.Update(patient);


                    //Return Status Code for result status. 
                    return Content("Success");
                }

                //return proper status code.
                return Content("Patient Updated");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }

        }

        /// <summary>
        /// Creates a Patient
        /// </summary>
        /// <param name="patientCreationBundle">The Patient Bundle to be created</param>
        /// <returns>A Response Code</returns>
        [HttpPost("Create")]
        public IActionResult CreatePatient(PatientCreationBundle patientCreationBundle)
        {
            try
            {
                Patient patient = patientCreationBundle.Patient;

                Auth auth = _authRepo.Read(a => a.Email.Equals(patient.Email, StringComparison.InvariantCultureIgnoreCase));

                if (!(auth is null))
                {
                    throw new Exception();
                }

                //Generate a Auth Entry
                Auth authEntry = new Auth()
                {
                    Email = patient.Email,
                    Password = patientCreationBundle.Password
                };

                //Generate a TokenAuth Entry
                TokenAuth tokenAuthEntry = new TokenAuth()
                {
                    Token = HashPassword(authEntry.Password)
                };

                //Assing the Appropriate Navigation Properties in TokenAuth
                tokenAuthEntry.Auth = authEntry;
                tokenAuthEntry.User = patient;

                //Assign a doctor if the token is not null.
                if (!(patientCreationBundle.DoctorToken is null))
                {
                    //Assign the appropriate doctor if found.
                    int userId = _tokenAuthRepo.Read(t => t.Token.Substring(t.Token.Length - 6, 6) == patientCreationBundle.DoctorToken).UserId;
                    patient.Doctor = _doctorRepo.Read(d => d.UserId == userId);
                    patient.Doctor.Patients.Add(patient);
                }
                
                //Create new Entries in the Patient, Auth and TokenAuth tables.
                _patientRepo.Create(patient);
                _authRepo.Create(authEntry);
                _tokenAuthRepo.Create(tokenAuthEntry);

                //Return proper status code.
                return Content("Patient Created");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        /// <summary>
        /// Deletes a Patient
        /// </summary>
        /// <param name="token">A User's Identity</param>
        /// <returns>A Response Code</returns>
        [HttpDelete("Delete")]
        public IActionResult DeletePatient([FromHeader(Name = "token")]string token)
        {
            try
            {
                //Find the user based on the token
                if (!(!(GrabUser(token).Value is Patient patient)))
                {
                    //Find the entries for the user based on the token
                    TokenAuth tokenAuth = _tokenAuthRepo.Read(ta => ta.UserId == patient.UserId);
                    Auth authEntry = _authRepo.Read(a => a.AuthId == tokenAuth.AuthId);

                    //Delete all Entries
                    _authRepo.Delete(authEntry);
                    _tokenAuthRepo.Delete(tokenAuth);
                    _patientRepo.Delete(patient);


                    foreach (var bs in patient.PatientBloodSugars)
                    {
                        _bloodSugarRepo.Delete(bs);
                    }

                    foreach (var pe in patient.PatientExercises)
                    {
                        _exerciseRepo.Delete(pe);
                    }

                    foreach (var pc in patient.PatientCarbs)
                    {
                        _carbRepo.Delete(pc);
                    }

                    //Empty variables
                    patient = null;
                    tokenAuth = null;
                    authEntry = null;

                    //Return proper status code.
                    return Content("Patient Deleted");
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }
        #endregion

    }
}
