using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class SystemSettingRepository : RepositoryBase<SystemSetting>, ISystemSettingRepository
    {
        public SystemSettingRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public SystemSetting GetBySystemSettingId(int systemSettingId)
        {
            return _dataContext.SystemSettings.Single(x => x.SettingId == systemSettingId);
        }
    }
}
