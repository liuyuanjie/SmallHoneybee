using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for UserWithMemberCard.xaml
    /// </summary>
    public partial class MemberCardInfo : Window
    {
        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;
        private ISaleOrderRepository _saleOrderRepository;
        private IMemberCardRepository _memberCardRepository;

        DataModel.Model.MemberCard _memberCard = new DataModel.Model.MemberCard();
        SavedMoneyModel _savedMoneyModel = new SavedMoneyModel();

        private readonly int _userId;
        private User _user;

        public MemberCardInfo(User user, int userId)
        {
            _user = user;
            _userId = userId;
            InitializeComponent();
            ExecuteSearchText();

            var howBalances = new List<KeyValuePair<sbyte?, string>>();
            howBalances.AddRange(CommonHelper.Enumerate<DataType.SaleOrderBalancedMode>()
                .Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            DataContext = new
            {
                MemberCard = _memberCard,
                SavedMoneyModel = _savedMoneyModel,
                LogTypeTexts = howBalances
            };
            ComboLogType.SelectedValue = (sbyte)DataType.SaleOrderBalancedMode.Cash;
            txtSavedMoney.Focus();
        }

        private void ExecuteSearchText()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _userRepository = _unitOfWork.GetRepository<UserRepository>();
            _memberCardRepository = _unitOfWork.GetRepository<MemberCardRepository>();
            _saleOrderRepository = _unitOfWork.GetRepository<SaleOrderRepository>();

            _memberCard =
                _memberCardRepository.Query()
                    .FirstOrDefault(x => x.IsEnable && x.RelateUserId == _userId);

            SetMemberCardInfo();
        }

        private void SetMemberCardInfo()
        {
            txtMemberCardNo.Text = _memberCard.MemberCardNo;
            txtMemberCardIsEnable.Text = CommonHelper.EnableTexts.Single(x => x.Key == _memberCard.IsEnable).Value;
            txtMemberCardName.Text = _memberCard.Name;
            txtMemberCardMemberMoney.Text = _memberCard.MemberMoney.ToString("F2");
            txtMemberCardMemberType.Text =
                CommonHelper.Enumerate<DataType.MemberType>().Single(x => x.Key == _memberCard.MemberType).Value;
            txtMemberCardSurplusMoney.Text = _memberCard.TotalSurplusMoney.ToString("F2");
        }

        private void ButSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataModel.Model.User bindingUser = _userRepository.GetByUserId(_userId);

                bindingUser.CashTotal = _memberCard.TotalSurplusMoney += _savedMoneyModel.SavedMoney + _savedMoneyModel.PresentedMoney;
                bindingUser.CashBalance = _memberCard.PrincipalSurplusMoney += _savedMoneyModel.SavedMoney;
                bindingUser.CashFee = _memberCard.FavorableSurplusMoney += _savedMoneyModel.PresentedMoney;

                _memberCard.MemberCardlogs.Add(new MemberCardLog
                {
                    FavorableMoney = _savedMoneyModel.PresentedMoney,
                    PrincipalMoney = _savedMoneyModel.SavedMoney,
                    LogType = (sbyte)DataType.MemberCardLogType.Saved,
                    ChangedBy = ResourcesHelper.CurrentUser.Name,
                    DateChanged = DateTime.Now,
                    NewValue = string.Format(ResourcesHelper.MemberCardLogFormat,
                        ComboLogType.Text,
                        bindingUser.Name,
                        _savedMoneyModel.SavedMoney.ToString("F2"),
                        _savedMoneyModel.PresentedMoney.ToString("F2"),
                        _memberCard.TotalSurplusMoney.ToString("F2"))
                });
                _memberCardRepository.Update(_memberCard);

                bindingUser.Userlogs.Add(new Userlog
                {
                    LogType = (sbyte)DataType.MemberCardLogType.Saved,
                    ChangedBy = ResourcesHelper.CurrentUser.Name,
                    DateChanged = DateTime.Now,
                    NewValue = string.Format(ResourcesHelper.UserLogMemberCardFormat,
                        ComboLogType.Text,
                        _savedMoneyModel.SavedMoney.ToString("F2"),
                        _savedMoneyModel.PresentedMoney.ToString("F2"),
                        _memberCard.TotalSurplusMoney.ToString("F2"))
                });
                _userRepository.Update(bindingUser);

                _unitOfWork.Commit();

                MessageBox.Show("充值成功！", Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log4NetHelper.WriteLog(ex.ToString());

                MessageBox.Show("充值失败！", Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _user.ExecuteSearchText();
            Close();
        }

        private void ButCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class SavedMoneyModel
    {
        public float SavedMoney { get; set; }
        public float PresentedMoney { get; set; }
    }
}
