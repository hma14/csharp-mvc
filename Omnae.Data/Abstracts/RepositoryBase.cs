using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace Omnae.Data.Abstracts
{
    public abstract class RepositoryBase<T> where T : class
    {
        private readonly IDbSet<T> dbSet;

        protected readonly OmnaeContext DbContext;
        

        protected RepositoryBase(OmnaeContext dbContext)
        {
            DbContext = dbContext;
            dbSet = DbContext.Set<T>();
        }

        #region Implementation
        public  void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Update(T entity)
        {            
            DbContext.Set<T>().AddOrUpdate(entity);
            DbContext.SaveChanges();
        }

        public virtual void Detach(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Detached;
        }

        public virtual void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbSet.Remove(obj);
        }

        public virtual T GetById(int id)
        {
            return dbSet.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault<T>();
        }

        public virtual List<TResult> ExecStoredProcedure<TResult>(string query, params object[] parameters)
        {
            return this.DbContext.Database.SqlQuery<TResult>(query, parameters).ToList();
        }

        #endregion
    }
}
