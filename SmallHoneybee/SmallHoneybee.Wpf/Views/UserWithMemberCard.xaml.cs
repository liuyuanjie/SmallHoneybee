using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
    public partial class UserWithMemberCard : Window
    {
        private IUnitOfWork _unitOfWork;
        private IUserRepository _userRepository;
        private ISaleOrderRepository _saleOrderRepository;
        private IMemberCardRepository _memberCardRepository;

        DataModel.Model.MemberCard _memberCard = new DataModel.Model.MemberCard();

        private readonly int _userId;
        private User _user;

        public UserWithMemberCard(User user, int userId)
        {
            _user = user;
            _userId = userId;
            InitializeComponent();
            ExecuteSearchText();
            DataContext = new
            {
                MemberCard = _memberCard,
            };
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

            if (_memberCard != null)
            {
                SetMemberCardInfo();

                txtMemberCardNo.IsEnabled = false;
                txtFirstPassWord.Focus();
            }
            else
            {
                txtMemberCardNo.Focus();
            }
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
            SaveUserWithMemberCard();
        }

        private void SaveUserWithMemberCard()
        {
            if (_memberCard != null)
            {
                DataModel.Model.User bindingUser = _userRepository.GetByUserId(_userId);

                if (_memberCard.RelateUserId.HasValue)
                {
                    try
                    {
                        if (!ComparePasswordEqual()) return;
                        SaltedHash saltedHash = SaltedHash.Create(txtFirstPassWord.Password);
                        _memberCard.PasswordHash = saltedHash.Hash;
                        _memberCard.PasswordSalt = saltedHash.Salt;

                        _memberCard.MemberCardlogs.Add(new MemberCardLog
                        {
                            ChangedBy = ResourcesHelper.CurrentUser.Name,
                            DateChanged = DateTime.Now,
                            NewValue = string.Format("会员: {0}与购物卡: {1}重新设置密码！", bindingUser.Name, _memberCard.MemberCardNo)
                        });

                        _memberCardRepository.Update(_memberCard);

                        _unitOfWork.Commit();

                        MessageBox.Show("密码设置成功！", Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        Log4NetHelper.WriteLog(ex.ToString());

                        MessageBox.Show("密码设置失败！", Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    try
                    {
                        if (!ComparePasswordEqual()) return;
                        SaltedHash saltedHash = SaltedHash.Create(txtFirstPassWord.Password);
                        _memberCard.PasswordHash = saltedHash.Hash;
                        _memberCard.PasswordSalt = saltedHash.Salt;
                        _memberCard.DispatchUserId = ResourcesHelper.CurrentUser.UserId;
                        _memberCard.RelateUserId = _userId;
                        _memberCard.MemberCardStatus = (sbyte) DataType.MemberCardStatus.Active;

                        _memberCard.MemberCardlogs.Add(new MemberCardLog
                        {
                            ChangedBy = ResourcesHelper.CurrentUser.Name,
                            DateChanged = DateTime.Now,
                            PrincipalMoney = _memberCard.PrincipalSurplusMoney,
                            FavorableMoney = _memberCard.FavorableSurplusMoney,
                            LogType = (sbyte)DataType.MemberCardLogType.Saved,
                            NewValue = string.Format(ResourcesHelper.MemberCardLogUserFormat,
                            bindingUser.Name,
                            _memberCard.MemberCardNo,
                            _memberCard.PrincipalSurplusMoney.ToString("F2"))
                        });
                        _memberCardRepository.Update(_memberCard);

                        DataModel.Model.User user = _userRepository.GetByUserId(_userId);
                        user.CashTotal = _memberCard.TotalSurplusMoney;
                        user.CashFee = _memberCard.FavorableSurplusMoney;
                        user.CashBalance = _memberCard.PrincipalSurplusMoney;
                        _userRepository.Update(user);

                        _unitOfWork.Commit();

                        MessageBox.Show("购物卡绑定成功！", Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        Log4NetHelper.WriteLog(ex.ToString());

                        MessageBox.Show("购物卡绑定失败！", Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                _user.ExecuteSearchText();
                Close();
            }
        }

        private void ButCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TxtFirstPassWord_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtSecondPassWord.Focus();
            }
        }

        private void TxtSecondPassWord_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveUserWithMemberCard();
            }
        }

        private bool ComparePasswordEqual()
        {
            if (!string.Equals(txtFirstPassWord.Password, txtSecondPassWord.Password))
            {
                MessageBox.Show("密码设置失败, 两次输入密码不一致, 请重新输入！", Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (txtFirstPassWord.Password.Length < 6)
            {
                MessageBox.Show("密码设置失败, 请输入长度为6位密码！", Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void TxtMemberCardNo_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                List<DataModel.Model.MemberCard> memberCards =
                    _memberCardRepository
                    .Query()
                    .Where(x => x.MemberCardNo == txtMemberCardNo.Text ||
                        x.MemberCardNo == (ResourcesHelper.MFTMemberCard + txtMemberCardNo.Text))
                    .ToList();
                if (memberCards.Count != 1)
                {
                    MessageBox.Show("购物卡编号不正确, 请重新输入！", Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    if (memberCards[0].RelateUserId.HasValue)
                    {
                        MessageBox.Show("购物卡已经关联其他用户, 请重新输入！", Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        _memberCard = memberCards[0];
                        SetMemberCardInfo();
                    }
                }
            }
        }
    }
}
