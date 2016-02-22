using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;

namespace SmallHoneybee.EF.Data.Impl
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDatabaseContext _databaseContext;
        private readonly SmallHoneybeeEntities _dataContext;
        private bool _disposed = false;

        public UnitOfWork(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            _dataContext = databaseContext.GetDbContext();
        }

        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            return
                UnityInit.UnityContainer.Resolve<TRepository>(new ParameterOverrides()
                {
                    {"databaseContext",_databaseContext}
                });
        }

        public void Commit()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }

            _dataContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing && _dataContext != null)
            {
                _dataContext.Dispose();
            }

            _disposed = true;
        }
    }
}
