using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Impl
{
    public class DatabaseContext : IDatabaseContext
    {
        private readonly SmallHoneybeeEntities _dbEntities;
        public DatabaseContext(SmallHoneybeeEntities dbEntities)
        {
            _dbEntities = dbEntities;
        }
        public SmallHoneybeeEntities GetDbContext()
        {
            return _dbEntities;
        }
    }
}
