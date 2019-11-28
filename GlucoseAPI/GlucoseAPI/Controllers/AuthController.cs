///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         AuthController.cs
//	Description:       API Interface for Grabbing Patient Tokens.
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Services;
using static BCrypt.Net.BCrypt;

namespace GlucoseAPI.Controllers
{
    /// <summary>
    /// API Interface for Grabbing Patient Tokens.
    /// </summary>
    [Route("API/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Dependency Injection
        private readonly IRepository<Auth> _authRepo;
        private readonly IRepository<TokenAuth> _tokenAuthRepo;
        private readonly IRepository<Patient> _patientRepo;

        /// <summary>
        /// Default Constructor. Dependency Injection
        /// </summary>
        /// <param name="authRepo">Dependency Injection</param>
        /// <param name="tokenAuthRepo">Dependency Injection</param>
        /// <param name="patientRepo">Dependency Injection</param>
        public AuthController(IRepository<Auth> authRepo, IRepository<TokenAuth> tokenAuthRepo, IRepository<Patient> patientRepo)
        {
            _authRepo = authRepo;
            _tokenAuthRepo = tokenAuthRepo;
            _patientRepo = patientRepo;
        }
        #endregion

        #region Login
        /// <summary>
        /// A Method for Returning a token for a User.
        /// </summary>
        /// <param name="creds">User Credentials</param>
        /// <returns>A Response based on input</returns>
        [HttpPost]
        public ActionResult<string> GrabToken(Credentials creds)
        {
            //Load the Auth Entry for the User
            Auth authorization = _authRepo.Read(c => c.Email.Equals(creds.Email, StringComparison.InvariantCultureIgnoreCase));

            if (authorization != null)
            {
                if (Verify(creds.Password, authorization.Password))
                {
                    //Load the Token Entry for the User
                    TokenAuth tokenEntry = _tokenAuthRepo.Read(t => t.AuthId == authorization.AuthId);

                    //If the user assigned to the Token Entry is a patient.... Log them in.m
                    if (_patientRepo.ReadAll().Any(p => p.UserId == tokenEntry.UserId))
                    {
                        return tokenEntry.Token;
                    }
                    else
                    {
                        return Content("Invalid Credentials");
                    }
                }
                else
                {
                    return Content("Invalid Credentials");
                }
            }
            else
            {
                return Content("Invalid Credentials");
            }
        }
        #endregion
    }
}
