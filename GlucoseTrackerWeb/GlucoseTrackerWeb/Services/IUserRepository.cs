using GlucoseTrackerWeb.Models.DBFEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseTrackerWeb.Services
{
    public interface IUserRepository
    {
        User Create(User user);

        User Read(string email);

        User Read(int id);

        ICollection<User> ReadAll();

        void Update(int id, User user);

        void Delete(int id);
    }
}
