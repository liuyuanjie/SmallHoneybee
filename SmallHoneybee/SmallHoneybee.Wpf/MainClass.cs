using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using SmallHoneybee.Common;
using SmallHoneybee.Wpf.Common;
using SmallHoneybee.Wpf.Properties;
using SmallHoneybee.Wpf.Views;

namespace SmallHoneybee.Wpf
{
    public class MainClass
    {
        [STAThread]
        static void Main(string[] args)
        {
            Log4NetHelper.SetConfig();

            SetCultureInfo();

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show(ResourcesHelper.ReRunSystemError, Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                RunProcess();
            }
        }

        static void RunProcess()
        {
            Application app = new Application();
            Login mainWindow = new Login();
            app.Run(mainWindow);
        }
    
        static void SetCultureInfo()
        {
            CultureInfo cultureInfo = new CultureInfo("zh-CN");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}
