using System.Collections.Generic;

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
        public static Dictionary<short, string> SystemSettings = new Dictionary<short, string>();
        public static string CoustomUserNoStart = "900000";

        public static RolePermission CurrentUserRolePermission;

    }
}
