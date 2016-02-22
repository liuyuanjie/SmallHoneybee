using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallHoneybee.EF.Data
{
    public interface IRepository<T> where T : class
    {
        void Delete(T entity);
        void Create(T entity);
        void Update(T entity);
        IQueryable<T> Query();
    }
}
