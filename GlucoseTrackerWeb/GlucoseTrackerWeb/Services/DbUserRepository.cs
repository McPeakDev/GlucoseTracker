using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GlucoseTrackerWeb.Models.Entities;
using static BCrypt.Net.BCrypt;

namespace GlucoseTrackerWeb.Services
{
    public class DbUserRepository : IDbRepository<User>
    {
        private GlucoseTrackerContext _db;

        public DbUserRepository(GlucoseTrackerContext db)
        {
            _db = db;
        }

        public User Create(User user)
        {
            user.Email = user.Email.Trim();
            user.Password = HashPassword(user.Password);
            user.FirstName = user.FirstName.Trim();
            user.MiddleName = user.MiddleName.Trim();
            user.LastName = user.LastName.Trim();
            user.PhoneNumber = user.PhoneNumber.Trim().Replace("-", " ");
            /*user.Doctor = new Doctor()
            {
                DoctorId = user.UserId,
                NumberOfPatients = 0
            };*/
            _db.User.Add(user);
            _db.SaveChanges();
            return user;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public User Read(string email)
        {
            return _db.User.FirstOrDefault(p => p.Email.Equals(email,StringComparison.InvariantCultureIgnoreCase));
        }

        public User Read(int id)
        {
            return _db.User.FirstOrDefault(p => p.UserId == id);
        }

        public ICollection<User> ReadAll()
        {
            return _db.User.ToList();
        }

        public void Update(int id, User user)
        {
            throw new NotImplementedException();
        }
    }
}
