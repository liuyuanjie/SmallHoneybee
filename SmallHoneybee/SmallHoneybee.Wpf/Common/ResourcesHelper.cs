using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.Wpf.Properties;

namespace SmallHoneybee.Wpf.Common
{
    public class ResourcesHelper
    {
        public static string SystemName = Resources.SystemName;
        public const string ReRunSystemError = "系统已经在运行,请先停止当前运行系统!";
        public static DataModel.Model.User CurrentUser;

    }
}
