using GlucoseAPI.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseTrackerWeb.Services
{
    public interface IDbRepository<T>
    {
        T Create(T entity);

        T Read(string email);

        T Read(int id);

        ICollection<T> ReadAll(int? id);

        void Update(int id, T obj);

        void Delete(int id);
    }
}
