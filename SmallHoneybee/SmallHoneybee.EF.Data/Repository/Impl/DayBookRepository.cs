using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class DayBookRepository : RepositoryBase<DayBook>, IDayBookRepository
    {
        public DayBookRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public DayBook GetByDayBookId(int dayBookId)
        {
            return _dataContext.DayBooks.Single(x => x.DayBookId == dayBookId);
        }
    }
}
