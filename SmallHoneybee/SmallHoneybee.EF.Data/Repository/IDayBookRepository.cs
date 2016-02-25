using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository
{
    public interface IDayBookRepository : IRepository<DayBook>
    {
        DayBook GetByDayBookId(int dayBookId);
    }
}
