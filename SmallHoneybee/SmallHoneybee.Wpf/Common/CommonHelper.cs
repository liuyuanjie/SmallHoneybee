using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.Wpf.Properties;

namespace SmallHoneybee.Wpf.Common
{
    public class CommonHelper
    {
        public static IEnumerable<KeyValuePair<byte, string>> Enumerate<T>()
        {
            return from object value in Enum.GetValues(typeof(T))
                   select new KeyValuePair<byte, string>(Convert.ToByte(value), Resources.ResourceManager.GetString(Enum.GetName(typeof(T), value)));
        }

        public static IEnumerable<KeyValuePair<bool, string>> EnableTexts = new List<KeyValuePair<bool, string>>
        {
                new KeyValuePair<bool, string>(true, "可用" ),
                new KeyValuePair<bool, string>(false, "不可用" )
        };

        public static IEnumerable<KeyValuePair<sbyte, string>> SexTexts = new List<KeyValuePair<sbyte, string>>
        {
                new KeyValuePair<sbyte, string>(0, "女" ),
                new KeyValuePair<sbyte, string>(1, "男" )
        };

        public static void AddCreatedOnAndDate<T>(User user, T dataModel) where T : class
        {
            PropertyInfo createBy = typeof(T).GetProperty("CreatedBy");

            if (createBy != null)
            {
                createBy.SetValue(dataModel, user.Name, null);
            }

            PropertyInfo createdOn = typeof(T).GetProperty("CreatedOn");

            if (createdOn != null)
            {
                createdOn.SetValue(dataModel, DateTime.Now, null);
            }

            PropertyInfo lastModifiedBy = typeof(T).GetProperty("LastModifiedBy");

            if (lastModifiedBy != null)
            {
                lastModifiedBy.SetValue(dataModel, user.Name, null);
            }

            PropertyInfo lastModifiedOn = typeof(T).GetProperty("LastModifiedOn");

            if (lastModifiedOn != null)
            {
                lastModifiedOn.SetValue(dataModel, DateTime.Now, null);
            }
        }

        public static void UpdateModifiedOnAndDate<T>(User user, T dataModel) where T : class
        {
            PropertyInfo lastModifiedBy = typeof(T).GetProperty("LastModifiedBy");

            if (lastModifiedBy != null)
            {
                lastModifiedBy.SetValue(dataModel, user.Name, null);
            }

            PropertyInfo lastModifiedOn = typeof(T).GetProperty("LastModifiedOn");

            if (lastModifiedOn != null)
            {
                lastModifiedOn.SetValue(dataModel, DateTime.Now, null);
            }
        }
    }
}
