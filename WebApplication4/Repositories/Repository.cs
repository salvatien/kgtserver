using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
 
namespace DogsServer.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected Microsoft.EntityFrameworkCore.DbSet<T> DbSet;

        public Repository(Microsoft.EntityFrameworkCore.DbContext dataContext)
        {
            DbSet = dataContext.Set<T>();
        }

        #region IRepository<T> Members

        public void Insert(T entity)
        {
            DbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public T GetById(int id)
        {
            return DbSet.Find(id);
        }

        #endregion
    }
}
