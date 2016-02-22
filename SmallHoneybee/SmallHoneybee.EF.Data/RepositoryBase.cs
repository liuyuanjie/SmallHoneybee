using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data
{
    public abstract class RepositoryBase<T> where T : class
    {
        protected readonly SmallHoneybeeEntities _dataContext;
        private readonly IDbSet<T> _dbSet;

        protected RepositoryBase(IDatabaseContext databaseContext)
        {
            _dataContext = databaseContext.GetDbContext();
            _dbSet = _dataContext.Set<T>();
        }

        public IQueryable<T> Query()
        {
            return _dbSet;
        }

        public void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
