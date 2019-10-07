using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlucoseTrackerWeb.Models.Entities;
using static BCrypt.Net.BCrypt;

namespace GlucoseTrackerWeb.Services
{
    public class DbDoctorRepository : IDbRepository<Doctor>
    {
        private GlucoseTrackerContext _db;

        public DbDoctorRepository(GlucoseTrackerContext db)
        {
            _db = db;
        }

        public Doctor Create(Doctor doctor)
        {
            doctor.Email = doctor.Email.Trim();
            doctor.Password = HashPassword(doctor.Password);
            doctor.FirstName = doctor.FirstName.Trim();
            doctor.MiddleName = doctor.MiddleName.Trim();
            doctor.LastName = doctor.LastName.Trim();
            doctor.PhoneNumber = doctor.PhoneNumber.Trim().Replace("-", " ");
            _db.Doctor.Add(doctor);

            _db.Login.Add(new Login()
            {
                Email = doctor.Email,
                Password = doctor.Password,
                UserId = doctor.UserId,
                User = doctor
            });
            _db.SaveChanges();
            return doctor;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Doctor Read(string email)
        {
            return _db.Doctor.FirstOrDefault(p => p.Email.Equals(email,StringComparison.InvariantCultureIgnoreCase));
        }

        public Doctor Read(int id)
        {
            return _db.Doctor.FirstOrDefault(d => d.DoctorId == id);
        }

        public ICollection<Doctor> ReadAll(int? id)
        {
            return _db.Doctor.ToList();
        }

        public void Update(int id, Doctor doctor)
        {
            throw new NotImplementedException();
        }
    }
}
