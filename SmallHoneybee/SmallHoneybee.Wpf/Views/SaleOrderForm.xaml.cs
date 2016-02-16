﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.ObjectBuilder2;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;
using SmallHoneybee.Wpf.Properties;
using Brush = System.Drawing.Brush;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SaleOrderForm.xaml
    /// </summary>
    public partial class SaleOrderForm : INotifyPropertyChanged
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaleOrderRepository _saleOrderRepository;
        private readonly ISOProduceRepository _soProduceRepository;
        private readonly IProduceRepository _produceRepository;
        private readonly IUserRepository _userRepository;

        public readonly SaleOrder SaleOrderWindow;
        private DataModel.Model.SaleOrder _saleOrder;

        private BalanceDomainModel _balanceDomainModel = new BalanceDomainModel();
        private DataModel.Model.User _user = new DataModel.Model.User();

        private ObservableCollection<SOProduceDomainModel> _soProduceDomainModels
                = new ObservableCollection<SOProduceDomainModel>();

        public ObservableCollection<SOProduceDomainModel> SOProduceDomainModels
        {
            get { return _soProduceDomainModels; }
            set
            {
                _soProduceDomainModels = value;
                NotifyPropertyChange("SOProduceDomainModels");
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

        public SaleOrderForm(SaleOrder saleOrderWindow, DataModel.Model.SaleOrder saleOrder)
        {
            InitializeComponent();
            TxtUserNoOrPhone.Focus();
            _unitOfWork = UnityInit.UnitOfWork;
            _saleOrderRepository = _unitOfWork.GetRepository<SaleOrderRepository>();
            _soProduceRepository = _unitOfWork.GetRepository<SOProduceRepository>();
            _produceRepository = _unitOfWork.GetRepository<ProduceRepository>();
            _userRepository = _unitOfWork.GetRepository<UserRepository>();

            SaleOrderWindow = saleOrderWindow;

            if (saleOrder.SaleOrderId > 0)
            {
                _saleOrder = _saleOrderRepository.Query()
                    .Single(x => x.SaleOrderId == saleOrder.SaleOrderId);
                _saleOrder.SOProduces.ForEach(x => _soProduceDomainModels.Add(new SOProduceDomainModel
                {
                    SOProduce = x,
                    CostPerUnit = x.CostPerUnit,
                    SOProduceTotal = (x.CostPerUnit ?? 0) * x.Quantity ?? 0
                }));
            }
            else
            {
                _saleOrder = saleOrder;
            }

            _balanceDomainModel = new BalanceDomainModel
            {
                TotalPrice = saleOrder.TotalCost ?? 0,
                DiscountPrice = saleOrder.FavorableCost ?? 0,
                CashTotal = _user.CashTotal,
            };

            DataContext = new
            {
                SaleOrder = _saleOrder,
                SOProduceDomainModels,
                CurrentUserRolePermission = ResourcesHelper.CurrentUserRolePermission,
                BalanceDomainModel = _balanceDomainModel,
            };
            TxtDiscount.Text = _balanceDomainModel.DiscountPrice.ToString();

            _soProduceDomainModels.CollectionChanged += (sender, e) => SetTotalNameberText();

            SetTotalNameberText();
            if (_saleOrder.SaleOrderStatus == (byte)DataType.SaleOrderStatus.Balanced)
            {
                ButSave.IsEnabled = false;
                ButBalance.IsEnabled = false;
            }

            RadBanlanceModeCard.Click += (s, e) =>
            {
                TabItemCard.IsSelected = true;
            };
            RadBanlanceModeCash.Click += (s, e) =>
            {
                TabItemCash.IsSelected = true;
            };
        }

        private void ButDeleteSaleOrder_Click(object sender, MouseButtonEventArgs e)
        {
            var soProduceDomainModel = GridSOProduces.SelectedItem as SOProduceDomainModel;
            if (soProduceDomainModel != null)
            {
                _soProduceDomainModels.Remove(soProduceDomainModel);
                if (soProduceDomainModel.SOProduce.SOProduceId > 0)
                {
                    _soProduceRepository.Delete(soProduceDomainModel.SOProduce);
                }
            }
        }

        private void ButSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSaleOrder();
            _unitOfWork.Commit();
            SaleOrderWindow.ExecuteSearchText();

            Close();
        }

        private void SaveSaleOrder(DataType.SaleOrderStatus saleOrderStatus = DataType.SaleOrderStatus.NotBalanced)
        {
            _soProduceDomainModels.ForEach(
                x =>
                {
                    //x.SOProduce.CostPerUnit = x.SOProduce.DiscountRate * x.SOProduce.Produce.RetailPrice;
                    x.SOProduce.RetailPrice = x.SOProduce.Produce.RetailPrice;
                    x.SOProduce.SOProduceStatusCategory = (sbyte)saleOrderStatus;
                });
            _saleOrder.ProduceCost = _soProduceDomainModels.Sum(x => x.SOProduce.Quantity * x.CostPerUnit);
            _saleOrder.TotalCost = (_saleOrder.OtherCost ?? 0) + _saleOrder.ProduceCost;
            _saleOrder.SaleOrderStatus = (sbyte)saleOrderStatus;

            if (_saleOrder.SaleOrderId == 0)
            {
                CommonHelper.AddCreatedOnAndDate(ResourcesHelper.CurrentUser, _saleOrder);
                _soProduceDomainModels.ForEach(x => CommonHelper.AddCreatedOnAndDate(ResourcesHelper.CurrentUser, x.SOProduce));
                _saleOrder.SOProduces = _soProduceDomainModels.Select(x => x.SOProduce).ToList();
                _saleOrderRepository.Create(_saleOrder);
            }
            else
            {
                CommonHelper.UpdateModifiedOnAndDate(ResourcesHelper.CurrentUser, _saleOrder);
                _soProduceDomainModels.ForEach(x => CommonHelper.AddCreatedOnAndDate(ResourcesHelper.CurrentUser, x.SOProduce));
                _saleOrder.SOProduces = _soProduceDomainModels.Select(x => x.SOProduce).ToList();
                _saleOrderRepository.Update(_saleOrder);
            }

            _saleOrder.SaleOrderStatus = (sbyte)saleOrderStatus;
        }


        private void TxtQuantity_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var soProduceDomainModel = GridSOProduces.SelectedItem as SOProduceDomainModel;
            if (soProduceDomainModel != null)
            {
                soProduceDomainModel.SOProduceTotal = (soProduceDomainModel.SOProduce.Quantity ?? 0) *
                    (soProduceDomainModel.SOProduce.CostPerUnit ?? 0);

                SetTotalNameberText();
            }
        }

        private void TxtDiscountRate_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var soProduceDomainModel = GridSOProduces.SelectedItem as SOProduceDomainModel;
            if (soProduceDomainModel != null)
            {
                if (soProduceDomainModel.SOProduce.Produce.LowestDiscountRate.HasValue)
                {
                    TextBox txtDiscoutRate = (TextBox)sender;

                    if (ResourcesHelper.CurrentUserRolePermission.SaleOrderFavorableCost)
                    {
                        if (soProduceDomainModel.SOProduce.DiscountRate <
                            soProduceDomainModel.SOProduce.Produce.LowestDiscountRate)
                        {
                            txtDiscoutRate.Background =
                                (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFFFBF");
                        }
                        else
                        {
                            txtDiscoutRate.Background =
                                (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFF0FDFC");
                        }
                    }
                    else if (ResourcesHelper.CurrentUserRolePermission.SaleOrderFavorableLimitCost)
                    {
                        if (soProduceDomainModel.SOProduce.DiscountRate <
                            soProduceDomainModel.SOProduce.Produce.LowestDiscountRate)
                        {
                            txtDiscoutRate.Background =
                                (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFA8A8");
                        }
                        else
                        {
                            txtDiscoutRate.Background =
                                (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFF0FDFC");
                        }
                    }
                    else
                    {
                        txtDiscoutRate.Background =
                            (System.Windows.Media.Brush)new BrushConverter().ConvertFromString("#FFF0FDFC");
                    }
                }

                soProduceDomainModel.CostPerUnit
                    = soProduceDomainModel.SOProduce.CostPerUnit
                    = soProduceDomainModel.SOProduce.Produce.RetailPrice *
                        soProduceDomainModel.SOProduce.DiscountRate;
                soProduceDomainModel.SOProduceTotal = (soProduceDomainModel.SOProduce.Quantity ?? 0) *
                   (soProduceDomainModel.SOProduce.CostPerUnit ?? 0);

                SetTotalNameberText();
            }
        }

        private void TxtCostPerUnit_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var soProduceDomainModel = GridSOProduces.SelectedItem as SOProduceDomainModel;
            if (soProduceDomainModel != null)
            {
                soProduceDomainModel.SOProduce.CostPerUnit = soProduceDomainModel.CostPerUnit;
                soProduceDomainModel.SOProduceTotal = (soProduceDomainModel.SOProduce.Quantity ?? 0) *
                                                      (soProduceDomainModel.SOProduce.CostPerUnit ?? 0);

                SetTotalNameberText();
            }
        }

        private void SetTotalNameberText()
        {
            _saleOrder.TotalCost = (_saleOrder.OtherCost ?? 0) +
                (_soProduceDomainModels.Where(x => x.SOProduce != null)
                    .Sum(x => x.SOProduce.Quantity * x.SOProduce.CostPerUnit) ?? 0);

            _saleOrder.ProduceTotalCount = _soProduceDomainModels.Where(x => x.SOProduce != null)
                        .Sum(x => x.SOProduce.Quantity ?? 0);

            _saleOrder.ProduceTotalOriginal = _soProduceDomainModels.Where(x => x.SOProduce != null)
                .Sum(x => x.SOProduce.Quantity * x.SOProduce.RetailPrice ?? 0);

            _saleOrder.ProduceTotalDiscount = _soProduceDomainModels.Where(x => x.SOProduce != null)
                .Sum(x => x.SOProduce.Quantity * (x.SOProduce.RetailPrice * (1 - x.SOProduce.DiscountRate)) ?? 0);

            TextTotalCount.Text = string.Format("数量： {0}，", (_saleOrder.ProduceTotalCount ?? 0).ToString("F2"));
            TextTotalOriginal.Text = string.Format("原价总计： {0}，", (_saleOrder.ProduceTotalOriginal ?? 0).ToString("F2"));
            TextTotalDiscount.Text = string.Format("折让金额： {0}，", (_saleOrder.ProduceTotalDiscount ?? 0).ToString("F2"));
            TextTotalNumber.Text = string.Format("折后合计： {0}", (_saleOrder.TotalCost ?? 0).ToString("F2"));

            UpdateBalanceData();
        }

        private void ButCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TxtOtherCost_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _saleOrder.OtherCost = float.Parse(TxtOtherCost.Text);
                SetTotalNameberText();
            }
        }

        private void TxtOtherCost_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _saleOrder.OtherCost = string.IsNullOrEmpty(TxtOtherCost.Text) ? 0 : float.Parse(TxtOtherCost.Text);
            SetTotalNameberText();
        }

        private void TxtBarCode_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var barCode = (TextBox)sender;
                if (barCode.IsFocused)
                {
                    var produce = _produceRepository.Query().Where(x => x.BarCode.StartsWith(barCode.Text)).FirstOrDefault();
                    if (produce != null)
                    {
                        var soProduceDomainModel = GridSOProduces.SelectedItem as SOProduceDomainModel;
                        if (soProduceDomainModel == null)
                        {
                            soProduceDomainModel = new SOProduceDomainModel
                            {
                                SOProduce = new SOProduce
                                {
                                    Produce = produce,
                                    ProduceId = produce.ProduceId,
                                    Quantity = 1,
                                    CostPerUnit = produce.RetailPrice * produce.DiscountRate,
                                    SaleOrder = _saleOrder,
                                    SaleOrderId = _saleOrder.SaleOrderId,
                                    RetailPrice = produce.RetailPrice,
                                    DiscountRate = produce.DiscountRate
                                },
                                CostPerUnit = produce.RetailPrice * produce.DiscountRate,
                                SOProduceTotal = produce.RetailPrice * produce.DiscountRate * 1,
                            };
                            _soProduceDomainModels.Add(soProduceDomainModel);
                        }
                    }

                    barCode.Text = string.Empty;
                }
            }
        }

        private void TxtUserNoOrPhone_OnTextChanged(object sender, TextChangedEventArgs e)
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
                UpdateBalanceData();
            }
        }

        private void TxtRealPrice_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateBalanceData();
            }
        }

        public void UpdateBalanceData()
        {
            _balanceDomainModel.TotalPrice = _saleOrder.TotalCost ?? 0;
            _balanceDomainModel.DiscountPrice = string.IsNullOrEmpty(TxtDiscount.Text) ? 0 : float.Parse(TxtDiscount.Text);
            _balanceDomainModel.RealPrice = string.IsNullOrEmpty(TxtRealPrice.Text) ? 0 : float.Parse(TxtRealPrice.Text);
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

        private void ButPrint_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void ButBalance_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var soProduceDomainModel in _soProduceDomainModels)
                {
                    if (soProduceDomainModel.SOProduce.Quantity < 0 &&
                        string.IsNullOrEmpty(soProduceDomainModel.SOProduce.Description))
                    {
                        MessageBox.Show(string.Format("商品'{0}'数量小于零，确认数量或填写备注说明。", soProduceDomainModel.SOProduce.Produce.Name),
                            Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else if (ResourcesHelper.CurrentUserRolePermission.SaleOrderFavorableLimitCost &&
                            soProduceDomainModel.SOProduce.DiscountRate <
                                soProduceDomainModel.SOProduce.Produce.LowestDiscountRate)
                    {
                        MessageBox.Show(string.Format("商品'{0}'折扣过低, 要赔钱了！", soProduceDomainModel.SOProduce.Produce.Name),
                            Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                if (_balanceDomainModel.DiscountPrice > 0)
                {
                    //if (!ResourcesHelper.CurrentUserRolePermission.SaleOrderFavorableCost &&
                    //    _balanceDomainModel.DiscountPrice > _balanceDomainModel.TotalPrice * 0.05)
                    //{
                    //    MessageBox.Show("结算失败, 优惠金额太多, 要赔钱卖了！", Properties.Resources.SystemName,
                    //        MessageBoxButton.OK, MessageBoxImage.Warning);
                    //    return;
                    //}

                    if (_balanceDomainModel.DiscountPrice > _balanceDomainModel.TotalPrice)
                    {
                        MessageBox.Show("结算失败, 优惠金额大于应收金额, 要赔钱卖了！", Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else if ((_balanceDomainModel.ReturnedPrice < 0))
                {
                    MessageBox.Show("结算失败, 实收金额小于应收金额, 要多找钱了！", Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_user.UserId > 0)
                {
                    if (RadBanlanceModeCard.IsChecked != null && (bool)RadBanlanceModeCard.IsChecked)
                    {
                        if (_balanceDomainModel.SurplusPrice < 0 ||
                            _user.CashTotal - _balanceDomainModel.ReceivedPrice < 0)
                        {
                            MessageBox.Show("结算失败, 会员卡余额不足, 请用充值或现金结算！", Properties.Resources.SystemName,
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
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

                    _saleOrder.PurchaseOrderUserId = _user.UserId == 0 ? (int?)null : _user.UserId;
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

                SaveSaleOrder(DataType.SaleOrderStatus.Balanced);
                _saleOrder.HowBalance = RadBanlanceModeCash.IsChecked.HasValue && RadBanlanceModeCash.IsChecked.Value
                    ? (SByte)DataType.SaleOrderBalancedMode.Cash
                    : RadBanlanceModeCard.IsChecked.HasValue && RadBanlanceModeCard.IsChecked.Value
                        ? (SByte)DataType.SaleOrderBalancedMode.MemberCard
                        : (SByte)DataType.SaleOrderBalancedMode.UnitUnionPay;

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

                MessageBox.Show("结算成功！", Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Information);

                SaleOrderWindow.ExecuteSearchText();
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("结算失败, 请重新新建零售记录！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                                 MessageBoxButton.OK, MessageBoxImage.Error);

                SaleOrderWindow.ExecuteSearchText();
                Close();
            }
        }

        private void TxtBarCode_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //var barCode = (TextBox)sender;
            //if (barCode.IsFocused && !string.IsNullOrEmpty(barCode.Text) && barCode.Text.Length >= 3)
            //{
            //    var produces = _produceRepository.Query().Where(x => x.BarCode.StartsWith(barCode.Text));
            //    if (produces.Count() == 1)
            //    {
            //        var produce = produces.First();
            //        var soProduceDomainModel = GridSOProduces.SelectedItem as SOProduceDomainModel;
            //        if (soProduceDomainModel == null)
            //        {
            //            soProduceDomainModel = new SOProduceDomainModel
            //            {
            //                SOProduce = new SOProduce
            //                {
            //                    Produce = produce,
            //                    ProduceId = produce.ProduceId,
            //                    Quantity = 1,
            //                    CostPerUnit = produce.RetailPrice,
            //                    SaleOrder = _saleOrder,
            //                    SaleOrderId = _saleOrder.SaleOrderId,
            //                    DiscountRate = produce.DiscountRate
            //                }
            //            };
            //            _soProduceDomainModels.Add(soProduceDomainModel);
            //        }
            //    }
            //}
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

        private float _totalPrice;
        public float TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                NotifyPropertyChange("TotalPrice");
            }
        }
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

    public class BaseWindow : Window
    {
        public void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
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

        public T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
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
    }
}
