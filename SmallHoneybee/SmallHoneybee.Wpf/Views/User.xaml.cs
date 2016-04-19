using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.ObjectBuilder2;
using SmallHoneybee.Common;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class User : Page, INotifyPropertyChanged
    {
        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;

        private ObservableCollection<UserModel> _users = new ObservableCollection<UserModel>();

        public ObservableCollection<UserModel> Users
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

            var sexTexts = new List<KeyValuePair<sbyte, string>>();
            sexTexts.AddRange(CommonHelper.SexTexts.Select(x => new KeyValuePair<sbyte, string>(x.Key, x.Value)));

            var memberTypeTexts = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            memberTypeTexts.AddRange(CommonHelper.Enumerate<DataType.MemberType>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            ExecuteSearchText();

            DataContext = new
            {
                UserTypes = userTypes,
                Users,
                EnableTexts = enableTxets,
                SexTexts = sexTexts,
                MemberTypeTexts = memberTypeTexts
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

        public void ExecuteSearchText()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _userRepository = _unitOfWork.GetRepository<UserRepository>();

            _users.Clear();
            _userRepository
                .Query()
                .Where(x => x.UserType == (sbyte)DataType.UserType.Coustom)
                .Where(x => x.UserNo.Contains(TxtSearchBox.Text) ||
                    x.Phone.Contains(TxtSearchBox.Text) ||
                    x.Name.Contains(TxtSearchBox.Text))
                .ToList()
                .ForEach(x => _users.Add(
                    new UserModel
                    {
                        User = x,
                        MemberCardNo = x.MemberCardsRelateUser.Any(y => y.IsEnable)
                            ? x.MemberCardsRelateUser.First(y => y.IsEnable).MemberCardNo
                            : string.Empty
                    }));

            InitBlankRow();
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

        private void ButDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var user = gridUsers.SelectedItem as UserModel;
            if (user != null)
            {
                _users.Remove(user);
                if (user.User.UserId > 0)
                {
                    _userRepository.Delete(user.User);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var phones = _users
                    .Where(x => x.User.Phone != null)
                    .GroupBy(x => x.User.Phone)
                    .Where(g => g.Count() > 1)
                    .Select(x => x.Key)
                    .ToList();
                if (phones.Count > 0)
                {
                    MessageBox.Show(string.Format("会员电话号码不唯一！\r\n{0}",
                            phones.JoinStrings("|")),
                        Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _users.Where(x => !string.IsNullOrEmpty(x.User.Name) &&
                    !string.IsNullOrEmpty(x.User.Phone)).ForEach(x =>
                {
                    if (x.User.UserId > 0)
                    {
                        CommonHelper.UpdateModifiedOnAndDate(ResourcesHelper.CurrentUser, _users);
                        _userRepository.Update(x.User);
                    }
                    else
                    {
                        _userRepository.Create(x.User);
                    }
                });

                _unitOfWork.Commit();

                MessageBox.Show("保存成功！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                ClearSearchText();
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

        private void TextName_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InitBlankRow();
            }
        }

        private void InitBlankRow()
        {
            for (int i = 0; i < 5; i++)
            {
                _users.Add(new UserModel
                {
                    User = new DataModel.Model.User
                    {
                        UserType = 0,
                        Enable = true,
                        Login = new Guid().ToString(),
                        PasswordHash = SaltedHash.Create("xislkfweorkdf").Hash,
                        PasswordSalt = SaltedHash.Create("xislkfweorkdf").Salt,
                        UserNo =
                            (int.Parse(_users.Max(x => x.User.UserNo) ?? ResourcesHelper.CoustomUserNoStart) + 1)
                                .ToString(),
                        CreatedBy = ResourcesHelper.CurrentUser.Name,
                        CreatedOn = DateTime.Now,
                        LastModifiedBy = ResourcesHelper.CurrentUser.Name,
                        LastModifiedOn = DateTime.Now,
                    },
                    MemberCardNo = string.Empty
                });
            }
        }

        private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                Control control = GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (control != null)
                {
                    control.Focus();
                }
            }
        }

        private T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild((prop), i) as DependencyObject;
                if (child == null)
                    continue;

                T castedProp = child as T;
                if (castedProp != null)
                    return castedProp;

                castedProp = GetFirstChildByType<T>(child);

                if (castedProp != null)
                    return castedProp;
            }
            return null;
        }

        private void ButUserWithMemberCard_Click(object sender, RoutedEventArgs e)
        {
            var user = gridUsers.SelectedItem as UserModel;
            if (user != null)
            {
                UserWithMemberCard userWithMemberCard = new UserWithMemberCard(this, user.User.UserId);
                userWithMemberCard.ShowDialog();
            }
        }

        private void ButMemberCardInfo_Click(object sender, RoutedEventArgs e)
        {
            var user = gridUsers.SelectedItem as UserModel;
            if (user != null)
            {
                if (_userRepository.GetByUserId(user.User.UserId).MemberCardsRelateUser.Any(x => x.IsEnable))
                {
                    MemberCardInfo memberCardInfo = new MemberCardInfo(this, user.User.UserId);
                    memberCardInfo.ShowDialog();
                }
                else
                {
                    MessageBox.Show("请先绑定购物卡！", Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void TextPhone_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InitBlankRow();
            }
        }

        private void ButShowLog_Click(object sender, RoutedEventArgs e)
        {
            var user = gridUsers.SelectedItem as UserModel;
            if (user != null)
            {
                SmallHoneybee.Wpf.Views.UserLog userLog = new SmallHoneybee.Wpf.Views.UserLog(user.User.UserId);
                userLog.ShowDialog();
            }
        }
    }

    public class UserModel
    {
        public DataModel.Model.User User { get; set; }
        public string MemberCardNo { get; set; }

    }
}
