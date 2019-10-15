using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlucoseAPI.Models.Entities;

namespace GlucoseTrackerWeb.Services
{
    public class DbPatientRepository : IDbRepository<Patient>
    {
        private GlucoseTrackerContext _db;

        public DbPatientRepository(GlucoseTrackerContext db)
        {
            _db = db;
        }

        public Patient Create(Patient entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Patient Read(string email)
        {
            return null;// _db.User.FirstOrDefault(p => p.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }

        public Patient Read(int id)
        {
            return _db.Patient.FirstOrDefault(p => p.PatientId == id);
        }

        public ICollection<Patient> ReadAll(int? id)
        {
            return _db.Patient.Where(d => d.DoctorId == id).ToList();
        }

        public void Update(int id, Patient patient)
        {
            throw new NotImplementedException();
        }
    }
}

