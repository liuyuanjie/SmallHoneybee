using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallHoneybee.Common
{
    public class Log4NetHelper
    {
        private static readonly log4net.ILog _loginfo = log4net.LogManager.GetLogger("loginfo");
        private static readonly log4net.ILog _logerror = log4net.LogManager.GetLogger("_logerror");

        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public static void SetConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.Configure(configFile);
        }

        public static void WriteLog(string info)
        {
            if (_loginfo.IsInfoEnabled)
            {
                _loginfo.Info(info);
            }
        }

        public static void WriteLog(string info, Exception se)
        {
            if (_logerror.IsErrorEnabled)
            {
                _logerror.Error(info, se);
            }
        } 
    }
}
