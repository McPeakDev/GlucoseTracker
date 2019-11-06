//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         MealController.cs
//	Description:       API Interface for CRUD operations on MealItems
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Net.Http;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlucoseAPI.Controllers
{
    /// <summary>
    /// API Interface for CRUD operations on MealItems 
    /// </summary>
    [Route("api/Meal")]
    [ApiController]
    public class MealController : ControllerBase
    {
        #region Dependency Injection
        private IRepository<MealItem> _mealRepo;
        private IRepository<TokenAuth> _tokenAuthRepo;
        private IRepository<Patient> _patientRepo;


        public MealController(IRepository<MealItem> mealRepo, IRepository<TokenAuth> tokenAuthRepo, IRepository<Patient> patientRepo)
        {
            _mealRepo = mealRepo;
            _tokenAuthRepo = tokenAuthRepo;
            _patientRepo = patientRepo;
        }
        #endregion

        /// <summary>
        /// Creates a new MealItem
        /// </summary>
        /// <param name="token">A User's Identity</param>
        /// <param name="mealItem">A MealItem</param>
        /// <returns>A Response Code</returns>
        [HttpPost("Create")]
        public ActionResult<StringContent> UpdatePatientData([FromHeader(Name = "token")]string token, MealItem mealItem)
        {
            try
            {
                Patient patient = GrabPatient(token);

                if(!(patient is null))
                {
                    _mealRepo.Create(mealItem);

                    //Return proper Response Code
                    return Content("Meal Item Created");
                }
                return Content("Invalid Token");
            }
            catch (Exception)
            {
                return Content("Invalid User");
            }
        }

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