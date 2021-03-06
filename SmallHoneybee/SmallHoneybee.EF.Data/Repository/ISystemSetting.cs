﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository
{
    public interface ISystemSettingRepository : IRepository<SystemSetting>
    {
        SystemSetting GetBySystemSettingId(int systemSettingId);
    }
}
