using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.Wpf.Properties;

namespace SmallHoneybee.Wpf.Common
{
    public class ResourcesHelper
    {
        public static string SystemName = Resources.SystemName;
        public const string ReRunSystemError = "系统已经在运行,请先停止当前运行系统!";
        public static User CurrentUser;
        public static string CoustomUserNoStart = "900000";


        public static RolePermission CurrentUserRolePermission;

        public static Visibility ConvertBoolToVisibility(bool flag)
        {
            return flag ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
