using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmallHoneybee.Common;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;
using SmallHoneybee.Wpf.Properties;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private readonly IUserRepository _userRepository;
        private readonly ISystemSettingRepository _systemSettingRepository;

        public Login()
        {
            InitializeComponent();
            _userRepository = UnityInit.UnitOfWork.GetRepository<UserRepository>();
            _systemSettingRepository = UnityInit.UnitOfWork.GetRepository<SystemSettingRepository>();

            TxtVersion.Text = string.Format("当前版本号 : V{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            CheckBoxLogin.IsChecked = !string.IsNullOrEmpty(Settings.Default.Login);
            TextLogin.Text = Settings.Default.Login;
            if (string.IsNullOrEmpty(TextLogin.Text))
            {
                TextLogin.Focus();
            }
            else
            {
                TextPassword.Focus();
            }
        }

        private void Ok_OnClick(object sender, RoutedEventArgs e)
        {
            LoginCheck();
        }

        private void LoginCheck()
        {
            string login = TextLogin.Text;
            string password = TextPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return;
            }

            DataModel.Model.User user = _userRepository.Query().SingleOrDefault(x => x.Login == login);
            if (user != null && SaltedHash.Create(user.PasswordSalt, user.PasswordHash).Verify(password))
            {
                ResourcesHelper.CurrentUserRolePermission = new RolePermission((DataType.UserType)user.UserType);
                ResourcesHelper.SystemSettings = _systemSettingRepository.Query()
                    .Where(x => x.IsEnable)
                    .ToList()
                    .ToDictionary(x => short.Parse(x.SettingCode), x => x.SettingValue);
                ResourcesHelper.CurrentUser = user;

                if (CheckBoxLogin.IsChecked.HasValue &&
                    CheckBoxLogin.IsChecked.Value)
                {
                    Settings.Default.Login = login;
                    Settings.Default.Save();
                }

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("用户名和密码不正确!", Properties.Resources.SystemName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CommandBinding_LoginButton_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(TextLogin.Text) && !string.IsNullOrEmpty(TextPassword.Password);
        }

        private void CommandBinding_LoginButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoginCheck();
        }
    }
}
