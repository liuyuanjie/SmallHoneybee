using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.ObjectBuilder2;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;
using SmallHoneybee.Wpf.Properties;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for BalanceFrom.xaml
    /// </summary>
    public partial class BalanceFrom : Window
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaleOrderRepository _saleOrderRepository;
        private readonly ISOProduceRepository _soProduceRepository;
        private readonly IProduceRepository _produceRepository;
        private readonly IUserRepository _userRepository;

        private readonly SaleOrderForm _saleOrderFormWindow;

        private DataModel.Model.SaleOrder _saleOrder;

        private BalanceDomainModel _balanceDomainModel = new BalanceDomainModel();
        private DataModel.Model.User _user = new DataModel.Model.User();

        public BalanceFrom(SaleOrderForm saleOrderFormWindow, IUnitOfWork unitOfWork, DataModel.Model.SaleOrder saleOrder)
        {
            InitializeComponent();

            _unitOfWork = unitOfWork;
            _saleOrderRepository = _unitOfWork.GetRepository<SaleOrderRepository>();
            _soProduceRepository = _unitOfWork.GetRepository<SOProduceRepository>();
            _produceRepository = _unitOfWork.GetRepository<ProduceRepository>();
            _userRepository = _unitOfWork.GetRepository<UserRepository>();

            _saleOrderFormWindow = saleOrderFormWindow;

            _saleOrder = saleOrder;
            _balanceDomainModel = new BalanceDomainModel
            {
                TotalPrice = saleOrder.TotalCost ?? 0,
                DiscountPrice = 0.0f,
                CashTotal = _user.CashTotal,
            };

            DataContext = new
            {
                BalanceDomainModel = _balanceDomainModel,
            };

            RadBanlanceModeCard.Click += (s, e) =>
            {
                TabItemCard.IsSelected = true;
            };
            RadBanlanceModeCash.Click += (s, e) =>
            {
                TabItemCash.IsSelected = true;
            };
        }

        private void ButSure_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((_balanceDomainModel.SurplusPrice > 0 || _balanceDomainModel.ReturnedPrice > 0))
                {
                    MessageBox.Show("付款失败！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (_user.UserId > 0)
                    {
                        if (RadBanlanceModeCard.IsChecked != null && (bool)RadBanlanceModeCard.IsChecked)
                        {
                            if (_balanceDomainModel.SurplusPrice < 0 ||
                                _user.CashTotal - _balanceDomainModel.ReceivedPrice < 0)
                            {
                                MessageBox.Show("会员卡余额不足！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                _user.CashBalance -= _balanceDomainModel.SurplusPrice;
                                if (_user.CashBalance < 0)
                                {
                                    _user.CashFee += _user.CashBalance;
                                    _user.CashBalance = 0;
                                }
                                _user.CashTotal = _user.CashBalance + _user.CashFee;
                            }
                        }

                        _saleOrder.OriginUserId = _user.UserId == 0 ? (int?)null : _user.UserId;
                        _user.MemberPoints += _balanceDomainModel.ReceivedPrice * Settings.Default.MemberPointsRate;
                        _user.Userlogs.Add(new Userlog
                        {
                            ChangedBy = ResourcesHelper.CurrentUser.Name,
                            DateChanged = DateTime.Now,
                            NewValue = string.Format("消费单据 : {0}, 本次消费: {1}, 本次累计积分: {2}, 剩余金额: {3}, 累计积分: {4}",
                            _saleOrder.SaleOrderNo,
                            _balanceDomainModel.ReceivedPrice.ToString("F2"),
                            _balanceDomainModel.CurrentMemberPoints.ToString("F2"),
                            (_balanceDomainModel.SurplusPrice > 0 ? _balanceDomainModel.SurplusPrice : 0).ToString("F2"),
                           (_balanceDomainModel.CurrentMemberPoints + _balanceDomainModel.MemberPoints).ToString("F2"))
                        });
                    }

                    _saleOrder.FavorableCost = _balanceDomainModel.DiscountPrice;
                    _saleOrder.TotalCost = _balanceDomainModel.ReceivedPrice;
                    _saleOrder.SOProduces.ForEach(x =>
                    {
                        x.Produce.Quantity -= (x.Quantity ?? 0);
                        x.Produce.Producelogs.Add(new Producelog
                        {
                            ChangedBy = ResourcesHelper.CurrentUser.Name,
                            DateChanged = DateTime.Now,
                            NewValue = string.Format("消费单据 : {0}, 本次扣除数量: {1}, 本次消费价格: {2}, 剩余数量: {3}",
                            _saleOrder.SaleOrderNo,
                            (x.Quantity ?? 0).ToString("F2"),
                            (x.CostPerUnit ?? 0).ToString("F2"),
                            x.Produce.Quantity.ToString("F2"))

                        });
                    });
                    _saleOrder.SaleOrderStatus = (sbyte)DataType.SaleOrderStatus.Balanced;
                    _unitOfWork.Commit();

                    MessageBox.Show("结算成功！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    _isClosedSaleOrderFormWindow = true;
                    Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("结算失败！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                                 MessageBoxButton.OK, MessageBoxImage.Error);
                _isClosedSaleOrderFormWindow = true;
                Close();
            }
        }

        private void ButCancel_OnClick(object sender, RoutedEventArgs e)
        {
            _isClosedSaleOrderFormWindow = false;
            this.Close();
        }

        private void UserNoOrPhone_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var barCode = (TextBox)sender;
            if (barCode.IsFocused && !string.IsNullOrEmpty(barCode.Text) && barCode.Text.Length >= 3)
            {
                var users = _userRepository.Query().Where(x => x.Phone.StartsWith(barCode.Text) ||
                    x.UserNo.StartsWith(barCode.Text));
                if (users.Count() == 1)
                {
                    _user = users.First();
                    _balanceDomainModel.CashTotal = _user.CashTotal;
                    _balanceDomainModel.MemberPoints = _user.MemberPoints;


                    _balanceDomainModel.UserBaseInfo = string.Format("姓名：{0} 电话：{1}", _user.Name, _user.Phone);
                    _balanceDomainModel.UserMemberInfo = string.Format("会员账户：剩余金额：{0}, 累计积分：{1}",
                        _user.CashTotal.ToString("F2"), _user.MemberPoints.ToString("F2"));

                    _balanceDomainModel.MemberInfo = string.Format("当前积分：{0}, 本次积分：{1}, 累计积分：{2}",
                        _balanceDomainModel.MemberPoints.ToString("F2"), _balanceDomainModel.CurrentMemberPoints.ToString("F2"),
                        (_balanceDomainModel.CurrentMemberPoints + _balanceDomainModel.MemberPoints).ToString("F2"));
                }
            }
        }

        private void TxtDiscount_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _balanceDomainModel.DiscountPrice = float.Parse(TxtDiscount.Text);
                _balanceDomainModel.RealPrice = float.Parse(TxtRealPrice.Text);
                _balanceDomainModel.ReceivedPrice = _balanceDomainModel.TotalPrice - _balanceDomainModel.DiscountPrice;
                _balanceDomainModel.ReturnedPrice = _balanceDomainModel.RealPrice - _balanceDomainModel.ReceivedPrice;
                _balanceDomainModel.SurplusPrice = _balanceDomainModel.CashTotal - _balanceDomainModel.ReceivedPrice;
                if (_user.UserId > 0)
                {
                    _balanceDomainModel.MemberInfo = string.Format("当前积分：{0}, 本次积分：{1}, 累计积分：{2}",
                       _balanceDomainModel.MemberPoints.ToString("F2"), _balanceDomainModel.CurrentMemberPoints.ToString("F2"),
                       (_balanceDomainModel.CurrentMemberPoints + _balanceDomainModel.MemberPoints).ToString("F2"));
                }
            }
        }

        private void TxtRealPrice_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _balanceDomainModel.DiscountPrice = float.Parse(TxtDiscount.Text);
                _balanceDomainModel.RealPrice = float.Parse(TxtRealPrice.Text);
                _balanceDomainModel.ReceivedPrice = _balanceDomainModel.TotalPrice - _balanceDomainModel.DiscountPrice;
                _balanceDomainModel.ReturnedPrice = _balanceDomainModel.RealPrice - _balanceDomainModel.ReceivedPrice;
                _balanceDomainModel.SurplusPrice = _balanceDomainModel.CashTotal - _balanceDomainModel.ReceivedPrice;
                if (_user.UserId > 0)
                {
                    _balanceDomainModel.MemberInfo = string.Format("当前积分：{0}, 本次积分：{1}, 累计积分：{2}",
                       _balanceDomainModel.MemberPoints.ToString("F2"), _balanceDomainModel.CurrentMemberPoints.ToString("F2"),
                       (_balanceDomainModel.CurrentMemberPoints + _balanceDomainModel.MemberPoints).ToString("F2"));
                }
            }
        }

        private void BalanceFrom_OnClosed(object sender, EventArgs e)
        {
            if (_isClosedSaleOrderFormWindow)
            {
                _saleOrderFormWindow.SaleOrderWindow.ExecuteSearchText();
                _saleOrderFormWindow.Close();
            }
        }

        private bool _isClosedSaleOrderFormWindow = false;
    }

    public class BalanceDomainModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _userBaseInfo;
        public string UserBaseInfo
        {
            get { return _userBaseInfo; }
            set
            {
                _userBaseInfo = value;
                NotifyPropertyChange("UserBaseInfo");
            }
        }

        private string _userMemberInfo;
        public string UserMemberInfo
        {
            get { return _userMemberInfo; }
            set
            {
                _userMemberInfo = value;
                NotifyPropertyChange("UserMemberInfo");
            }
        }


        private float _discountPrice;
        public float DiscountPrice
        {
            get { return _discountPrice; }
            set
            {
                _discountPrice = value;
                NotifyPropertyChange("DiscountPrice");
            }
        }

        //private float _receivedPrice;
        public float ReceivedPrice
        {
            get { return TotalPrice - DiscountPrice; }
            set
            {
                //_receivedPrice = value;
                NotifyPropertyChange("ReceivedPrice");
            }
        }

        private float _realPrice;
        public float RealPrice
        {
            get { return _realPrice; }
            set
            {
                _realPrice = value;
                NotifyPropertyChange("RealPrice");
            }
        }

        //private float _returnedPrice;
        public float ReturnedPrice
        {
            get { return RealPrice - ReceivedPrice; }
            set
            {
                //_returnedPrice = value;
                NotifyPropertyChange("ReturnedPrice");
            }
        }

        private float _detuctedPrice;

        public float DetuctedPrice
        {
            get { return _detuctedPrice; }
            set
            {
                _detuctedPrice = value;
                NotifyPropertyChange("DetuctedPrice");
            }
        }

        //private float _surplusPrice;

        public float SurplusPrice
        {
            get { return CashTotal - ReceivedPrice; }
            set
            {
                //_surplusPrice = value;
                NotifyPropertyChange("SurplusPrice");
            }
        }

        private string _memberInfo;

        public string MemberInfo
        {
            get { return _memberInfo; }
            set
            {
                _memberInfo = value;
                NotifyPropertyChange("MemberInfo");
            }
        }

        public float TotalPrice { get; set; }

        public float CashTotal { get; set; }
        public float MemberPoints { get; set; }

        public float CurrentMemberPoints
        {
            get
            {
                return ReceivedPrice * Settings.Default.MemberPointsRate;
            }
        }
    }
}
