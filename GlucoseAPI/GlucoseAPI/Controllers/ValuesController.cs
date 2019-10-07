using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlucoseAPI.Models.Entities;
using GlucoseAPI.Models;
using static BCrypt.Net.BCrypt;

namespace GlucoseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly GlucoseTrackerContext _context;

        public ValuesController(GlucoseTrackerContext context)
        {
            _context = context;
        }

        private async Task<ActionResult<Patient>> GrabPatient(Login login)
        {
            var user = await _context.Patient.FindAsync(login.UserId);
            if (user != null && user is Patient)
            {
                return (Patient) user;
            }
            else
            {
                NotFound();
                return null;
            }
                
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        [HttpPost("Create")]
        public async void CreatePatient(Patient patient)
        {
            patient.Doctor = (Doctor) _context.Doctor.FirstOrDefault(d => d.UserId == patient.DoctorId);
            _context.Patient.Add(patient);
            _context.Login.Add(new Login()
            {
                Email = patient.Email,
                Password = patient.Password,
                UserId = patient.UserId

            });
            _context.SaveChanges();
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Credentials creds)
        {

            Login login = await _context.Login.FirstOrDefaultAsync(c => c.Email == creds.Email);

            return await GrabPatient(login);
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> DeletePatient(int id)
        {
            var user = await _context.Patient.FindAsync(id);
            if (user is Patient)
            {
                return NotFound();
            }

            _context.Patient.Remove(user);
            await _context.SaveChangesAsync();

            return null;
        }

        private bool UserExists(int id)
        {
            return _context.Patient.Any(e => e.UserId == id);
        }
    }
}
