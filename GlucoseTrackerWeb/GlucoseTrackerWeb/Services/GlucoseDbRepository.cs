using GlucoseAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GlucoseAPI.Services
{
    public class GlucoseDbRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private GlucoseTrackerContext _db;
        private DbSet<TEntity> _table;

        public GlucoseDbRepository(GlucoseTrackerContext db)
        {
            _db = db;
            _table = _db.Set<TEntity>();
        }
        public TEntity Create(TEntity obj)
        {
            _table.Add(obj);
            Save();
            return obj;
        }

        public TEntity Read(Func<TEntity, bool> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            //return _table.Include().Find(id);
            return includes.Aggregate(_table.AsQueryable(), (current, includeProperty) => current.Include(includeProperty)).FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> ReadAll(params Expression<Func<TEntity, object>>[] includes)
        {
            return includes.Aggregate(_table.AsQueryable(), (current, includeProperty) => current.Include(includeProperty));
        }

        public void Update(TEntity obj)
        {
            _table.Attach(obj);
            _db.Entry(obj).State = EntityState.Modified;
            Save();
        }

        public void Delete(TEntity obj)
        {
            _db.Set<TEntity>().Remove(obj);
            Save();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
