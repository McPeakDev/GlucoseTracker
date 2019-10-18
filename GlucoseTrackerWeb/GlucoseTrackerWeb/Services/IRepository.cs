using System;
using System.Linq;
using System.Linq.Expressions;

namespace ProMan.Services
{
    public interface IRepository<TEntity>
    {
        TEntity Read(Func<TEntity, bool> predicate, params Expression<Func<TEntity, object>>[] includes);
        IQueryable<TEntity> ReadAll(params Expression<Func<TEntity, object>>[] includes);
        TEntity Create(TEntity obj);
        void Update(TEntity obj);
        void Delete(TEntity obj);
    }
}
