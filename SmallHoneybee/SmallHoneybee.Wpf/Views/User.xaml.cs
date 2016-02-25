using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.ObjectBuilder2;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;
using SmallHoneybee.Wpf.Views.Shared;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class User : Page, INotifyPropertyChanged
    {
        private IUnitOfWork _unitOfWork;
        private  IUserRepository _userRepository;

        private ObservableCollection<DataModel.Model.User> _users = new ObservableCollection<DataModel.Model.User>();

        public ObservableCollection<DataModel.Model.User> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                NotifyPropertyChange("Users");
            }
        }

        public User()
        {
            InitializeComponent();

            SetInitData();
        }

        private void SetInitData()
        {
            var units = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            units.AddRange(CommonHelper.Enumerate<DataType.ProduceUnit>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            var enableTxets = new List<KeyValuePair<bool?, string>> { new KeyValuePair<bool?, string>(null, string.Empty) };
            enableTxets.AddRange(CommonHelper.EnableTexts.Select(x => new KeyValuePair<bool?, string>(x.Key, x.Value)));

            var userTypes = new List<KeyValuePair<sbyte, string>>();
            userTypes.AddRange(CommonHelper.Enumerate<DataType.UserType>().Select(x => new KeyValuePair<sbyte, string>((sbyte)x.Key, x.Value)));

            ExecuteSearchText();

            DataContext = new
            {
                UserTypes = userTypes, Users,
                EnableTexts = enableTxets
            };
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearSearchText();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSearchText();
        }

        private void ClearSearchText()
        {
            TxtSearchBox.Text = string.Empty;
        }

        private void ExecuteSearchText()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _userRepository = _unitOfWork.GetRepository<UserRepository>();

            _users.Clear();
            _userRepository
                .Query()
                .Where(x => x.UserNo.Contains(TxtSearchBox.Text) ||
                    x.Phone.Contains(TxtSearchBox.Text) ||
                    x.Name.Contains(TxtSearchBox.Text))
                .ToList()
                .ForEach(x => _users.Add(x));
        }

        private void CommandBinding_ClearSearchText_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TxtSearchBox.IsFocused;
        }

        private void CommandBinding_ExecuteSearchText_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = TxtSearchBox.IsFocused;
        }

        private void CommandBinding_ClearSearchText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearSearchText();
        }

        private void CommandBinding_ExecuteSearchText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteSearchText();
        }

        private void ButDeleteUser_Click(object sender, MouseButtonEventArgs e)
        {
            var user = gridUsers.SelectedItem as DataModel.Model.User;
            if (user != null)
            {
                _users.Remove(user);
                if (user.UserId > 0)
                {
                    _userRepository.Delete(user);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _users.Where(x => !string.IsNullOrEmpty(x.Name) &&
                    !string.IsNullOrEmpty(x.Phone)).ForEach(x =>
                {
                    if (x.UserId > 0)
                    {
                        CommonHelper.UpdateModifiedOnAndDate(ResourcesHelper.CurrentUser, _users);
                        _userRepository.Update(x);
                    }
                    else
                    {
                        _userRepository.Create(x);
                    }
                });

                _unitOfWork.Commit();

                MessageBox.Show("保存成功！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                 MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void TextUserNo_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _users.Add(new DataModel.Model.User
                {
                    UserType = 1,
                    Enable = true,
                    Login = new Guid().ToString(),
                    PasswordHash = SaltedHash.Create("xislkfweorkdf").Hash,
                    PasswordSalt = SaltedHash.Create("xislkfweorkdf").Salt,
                    UserNo = (int.Parse(_users.Max(x => x.UserNo)) + 1).ToString(),
                    CreatedBy = ResourcesHelper.CurrentUser.Name,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = ResourcesHelper.CurrentUser.Name,
                    LastModifiedOn = DateTime.Now,

                });
            }
        }
    }
}
