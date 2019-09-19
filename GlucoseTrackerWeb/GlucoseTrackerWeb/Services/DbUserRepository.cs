using System;
using System.Collections.Generic;
using System.Linq;
using GlucoseTrackerWeb.Models.DBFEntities;
using static BCrypt.Net.BCrypt;

namespace GlucoseTrackerWeb.Services
{
    public class DbUserRepository : IUserRepository
    {
        private GlucoseTrackerContext _db;

        public DbUserRepository(GlucoseTrackerContext db)
        {
            _db = db;
        }

        public User Create(User user)
        {
            string hashed = HashPassword(user.Password);
            user.Password = hashed;
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
