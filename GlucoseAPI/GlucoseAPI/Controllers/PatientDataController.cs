///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         PatientDataController.cs
//	Description:       API Interface for CRUD operations on Patient Data
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlucoseAPI.Models.Entities;
using System;
using System.Net.Http;
using GlucoseAPI.Services;

namespace GlucoseAPI.Controllers
{
    /// <summary>
    /// API Interface for CRUD operations on Patient Data
    /// </summary>
    [Route("API/Data")]
    [ApiController]
    public class PatientDataController : ControllerBase
    {
        #region Dependency Injection
        private readonly IRepository<MealItem> _mealRepo;
        private readonly IRepository<PatientBloodSugar> _bloodSugarRepo;
        private readonly IRepository<PatientExercise> _exerciseRepo;
        private readonly IRepository<PatientCarbohydrates> _carbRepo;
        private readonly IRepository<Patient> _patientRepo;
        private readonly IRepository<TokenAuth> _tokenAuthRepo;



        public PatientDataController(IRepository<MealItem> mealRepo, IRepository<PatientBloodSugar> bloodSugarRepo, IRepository<PatientExercise> exerciseRepo, IRepository<PatientCarbohydrates> carbRepo, IRepository<Patient> patientRepo, IRepository<TokenAuth> tokenAuthRepo)
        {
            _mealRepo = mealRepo;
            _bloodSugarRepo = bloodSugarRepo;
            _exerciseRepo = exerciseRepo;
            _carbRepo = carbRepo;
            _patientRepo = patientRepo;
            _tokenAuthRepo = tokenAuthRepo;
        }
        #endregion

        #region Patient Data
        /// <summary>
        /// Reads Data for the given Patient
        /// </summary>
        /// <param name="token">A User's Identity</param>
        /// <returns>A Patient Data Structure</returns>
        [HttpPost("Read")]
        public ActionResult<PatientData> GrabPatientData([FromHeader(Name = "token")]string token)
        {
            try
            {
                PatientData patientData = new PatientData();

                Patient patient = GrabPatient(token);

                if (!(patient is null))
                {
                    patientData.PatientCarbohydrates = _carbRepo.ReadAll(pc => pc.Meal).Where(pc => pc.UserId == patient.UserId).ToList();
                    patientData.PatientExercises = _exerciseRepo.ReadAll().Where(pe => pe.UserId == patient.UserId).ToList();
                    patientData.PatientBloodSugars = _bloodSugarRepo.ReadAll(bs => bs.Meal).Where(bs => bs.UserId == patient.UserId).ToList();

                    return patientData;
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        /// <summary>
        /// Updates A Patient
        /// </summary>
        /// <param name="token">A User's Identity</param>
        /// <param name="patientData">A Patient's Data</param>
        /// <returns>A Response Code</returns>
        [HttpPut("Update")]
        public ActionResult<StringContent> UpdatePatientData([FromHeader(Name = "token")]string token, PatientData patientData)
        {
            try
            {
                Patient patient = GrabPatient(token);

                //Verify and Update the Patient
                if (!(patient is null))
                {
                    foreach (var bs in patientData.PatientBloodSugars)
                    {
                        _bloodSugarRepo.Update(bs);
                    }

                    foreach (var pe in patientData.PatientExercises)
                    {
                        _exerciseRepo.Update(pe);
                    }

                    foreach (var pc in patientData.PatientCarbohydrates)
                    {
                        _carbRepo.Update(pc);
                    }

                    //Return proper Response Code
                    return Content("Patient Data Updated");
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        /// <summary>
        /// Creates new Patient Data.
        /// </summary>
        /// <param name="token">A User's Identity</param>
        /// <param name="patientData">A Patient's Data</param>
        /// <returns>A Response Code</returns>
        [HttpPost("Create")]
        public ActionResult<StringContent> CreatePatientData([FromHeader(Name = "token")]string token, PatientData patientData)
        {
            try
            {
                Patient patient = GrabPatient(token);

                //Verify and Create new Patient Data
                if (!(patient is null))
                {
                    foreach (var bs in patientData.PatientBloodSugars)
                    {
                        _bloodSugarRepo.Create(bs);
                        //TODO: Add Meal Items into Database. Querying the USDA Database will need to be supplied on the phone side.
                    }

                    foreach (var pe in patientData.PatientExercises)
                    {
                        _exerciseRepo.Create(pe);
                    }

                    foreach (var pc in patientData.PatientCarbohydrates)
                    {
                        _carbRepo.Create(pc);
                    }

                    //Return the proper response code.
                    return Content("Patient Data Created");
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

        /// <summary>
        /// Deletes the Patients's Data
        /// </summary>
        /// <param name="token">A User's Identity</param>
        /// <param name="patientData">A Patient's Data</param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public ActionResult<StringContent> DeletePatientData([FromHeader(Name = "token")]string token, PatientData patientData)
        {
            try
            {
                Patient patient = GrabPatient(token);

                //Verify and Delete the Patient's Data
                if (!(patient is null))
                {
                    foreach (var bs in patientData.PatientBloodSugars)
                    {
                        _bloodSugarRepo.Delete(bs);
                    }

                    foreach (var pe in patientData.PatientExercises)
                    {
                        _exerciseRepo.Delete(pe);
                    }

                    foreach (var pc in patientData.PatientCarbohydrates)
                    {
                        _carbRepo.Delete(pc);
                    }

                    //Return the Proper Response Code.
                    return Content("Patient Data Deleted");
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Grabs a Patient
        /// </summary>
        /// <param name="token">A User's Identity</param>
        /// <returns>A Patient</returns>
        public Patient GrabPatient(string token)
        {
            int userId = _tokenAuthRepo.Read(a => a.Token == token).UserId;

            Patient patient = _patientRepo.Read(p => p.UserId == userId);

            if (!(patient is null))
            {
                return patient;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}