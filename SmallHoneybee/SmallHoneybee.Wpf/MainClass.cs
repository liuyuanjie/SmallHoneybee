using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using MySql.Data.MySqlClient;
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

            RunUpdateSript();
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
            app.DispatcherUnhandledException += (s, e) =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("应用程序出现了未捕获的异常，{0}/n", e.Exception.Message);
                if (e.Exception.InnerException != null)
                {
                    stringBuilder.AppendFormat("/n {0}", e.Exception.InnerException.Message);
                }
                stringBuilder.AppendFormat("/n {0}", e.Exception.StackTrace);
                Log4NetHelper.WriteLog(stringBuilder.ToString());
                MessageBox.Show(stringBuilder.ToString());
                e.Handled = true;
            };
            Login mainWindow = new Login();
            app.Run(mainWindow);
        }

        static void SetCultureInfo()
        {
            CultureInfo cultureInfo = new CultureInfo("zh-CN");
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            cultureInfo.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
        }

        static void RunUpdateSript()
        {
            try
            {
                ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings["SmallHoneybeeAdoNet"];
                string dbVersion;
                using (MySqlConnection mySqlConnection = new MySqlConnection(settings.ConnectionString))
                {
                    MySqlCommand command = mySqlConnection.CreateCommand();
                    command.CommandText = "SELECT SettingValue FROM SystemSetting WHERE SettingCode = 1000";
                    mySqlConnection.Open();
                    dbVersion = command.ExecuteScalar().ToString();
                }

                DirectoryInfo folder = new DirectoryInfo(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, @"SmallHonebee.SQL"));

                IEnumerable<string> updateScripts = folder.GetFiles("Update_*.sql")
                    .Where(x => String.CompareOrdinal(x.Name.Split('-')[0], "Update_" + dbVersion) >= 0)
                    .OrderBy(x => x.Name).Select(x => x.Name);
                foreach (var fileName in updateScripts)
                {
                    string sqlScript;
                    using (StreamReader streamReader = new StreamReader(
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmallHonebee.SQL", fileName), Encoding.UTF8))
                    {
                        sqlScript = streamReader.ReadToEnd();
                        streamReader.Close();
                    }

                    using (MySqlConnection mySqlConnection = new MySqlConnection(settings.ConnectionString))
                    {
                        mySqlConnection.Open();
                        using (MySqlCommand cmd = mySqlConnection.CreateCommand())
                        using (MySqlTransaction xt = mySqlConnection.BeginTransaction())
                        {
                            cmd.Connection = mySqlConnection;
                            cmd.Transaction = xt;
                            try
                            {
                                cmd.CommandText = sqlScript;
                                cmd.ExecuteNonQuery();
                                xt.Commit();
                            }
                            catch (Exception ex)
                            {
                                xt.Rollback();
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.WriteLog(ex.ToString());
            }
        }
    }
}
