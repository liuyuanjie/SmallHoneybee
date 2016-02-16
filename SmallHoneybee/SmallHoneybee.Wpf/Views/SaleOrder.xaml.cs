using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
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
using DataGrid = System.Windows.Controls.DataGrid;
using MessageBox = System.Windows.MessageBox;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SaleOrder.xaml
    /// </summary>
    public partial class SaleOrder : Page
    {
        private IUnitOfWork _unitOfWork;
        private ISaleOrderRepository _saleOrderRepository;

        private ObservableCollection<DataModel.Model.SaleOrder> _saleOrders
            = new ObservableCollection<DataModel.Model.SaleOrder>();
        private ObservableCollection<SOProduceDomainModel> _soProduceDomainModels
            = new ObservableCollection<SOProduceDomainModel>();

        public SaleOrder()
        {
            InitializeComponent();

            SetInitData();
        }

        private void SetInitData()
        {
            DateStartDate.SelectedDate = DateTime.Today.AddDays(-(byte)DateTime.Today.DayOfWeek);
            DateEndDate.SelectedDate = DateTime.Today.AddDays(7 - (byte)DateTime.Today.DayOfWeek - 1);
            ExecuteSearchText();

            var saleOrderStatuses = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            saleOrderStatuses.AddRange(CommonHelper.Enumerate<DataType.SaleOrderStatus>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            var howBalances = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            howBalances.AddRange(CommonHelper.Enumerate<DataType.SaleOrderBalancedMode>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            DataContext = new
            {
                SaleOrders = _saleOrders,
                SOProduceDomainModels = _soProduceDomainModels,
                SaleOrderStatusTexts = saleOrderStatuses,
                HowBalanceTexts = howBalances
            };
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearSearchText();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSearchText();
            if (GridSaleOrders.Items.Count > 0)
            {
                GridSaleOrders.SelectedIndex = 0;
            }
        }

        private void ClearSearchText()
        {
            TxtSearchBox.Text = string.Empty;
        }

        public void ExecuteSearchText()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _saleOrderRepository = _unitOfWork.GetRepository<SaleOrderRepository>();

            _saleOrders.Clear();
            IQueryable<DataModel.Model.SaleOrder> saleOrders = _saleOrderRepository.Query();
            if (ResourcesHelper.CurrentUser.UserType < (byte) DataType.UserType.FactoryPriceManger)
            {
                saleOrders =
                    saleOrders.Where(x => x.OriginUserUser.UserType < (byte) DataType.UserType.FactoryPriceManger);
            }

            DateTime? startDate = DateStartDate.SelectedDate;
            DateTime? endDate = DateEndDate.SelectedDate;

            if (startDate.HasValue && endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);
                saleOrders = saleOrders.Where(x => x.DateOriginated >= startDate &&
                    x.DateOriginated < endDate);
            }
            else if (startDate.HasValue)
            {
                saleOrders = saleOrders.Where(x => x.DateOriginated >= startDate);
            }
            else if (endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);

                saleOrders = saleOrders.Where(x => x.DateOriginated < endDate);
            }

            saleOrders
                .OrderByDescending(x => x.DateOriginated)
                .Where(x => x.SaleOrderNo.Contains(TxtSearchBox.Text) ||
                    x.Name.Contains(TxtSearchBox.Text))
                .ToList()
                .ForEach(x => _saleOrders.Add(x));

            var totalInfo = _saleOrders.Where(x => x.SaleOrderStatus == (byte)DataType.SaleOrderStatus.Balanced)
                .Select(x => new { x, Group1 = 1 })
                .GroupBy(x => x.Group1)
                .Select(m => new
                {
                    TotalCount = m.Count(),
                    TotalQuantity = m.Sum(y => y.x.SOProduces.Sum(s => s.Quantity)),
                    TotalPrice = m.Sum(y => y.x.TotalCost),
                }).FirstOrDefault();
            TxtTotalInfo.Text = string.Format("共计 {0} 条记录, 共计 {1} 件商品，总计 {2}",
                totalInfo != null ? totalInfo.TotalCount : 0,
                (totalInfo != null ? totalInfo.TotalQuantity ?? 0.0F : 0.0F).ToString("F2"),
                (totalInfo != null ? totalInfo.TotalPrice ?? 0.0F : 0.0F).ToString("F2"));
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

        private void GridSaleOrders_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _soProduceDomainModels.Clear();

            DataModel.Model.SaleOrder saleOrder = (DataModel.Model.SaleOrder)((DataGrid)sender).CurrentItem;
            if (saleOrder != null)
            {
                saleOrder.SOProduces.ForEach(x => _soProduceDomainModels.Add(new SOProduceDomainModel
                {
                    SOProduce = x
                }));
            }
            else
            {
                if (((DataGrid)sender).Items.Count > 0)
                {
                    saleOrder = (DataModel.Model.SaleOrder)((DataGrid)sender).Items[0];
                    saleOrder.SOProduces.ForEach(x => _soProduceDomainModels.Add(new SOProduceDomainModel
                    {
                        SOProduce = x
                    }));
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int userId = ResourcesHelper.CurrentUser.UserId;
            DateTime startDateTime = DateTime.Now.Date;
            DateTime endDateTime = DateTime.Now.Date.AddDays(1);
            int maxTodayId = _saleOrderRepository.Query()
                .Count(x => x.DateOriginated >= startDateTime && x.DateOriginated < endDateTime) + 1;
            string saleOrderNo = string.Format("{0}{1}{2}",
                DateTime.Now.ToString("yyyyMMddHH"),
                userId,
                maxTodayId.ToString().PadLeft(4, '0'));

            var saleOrderForm = new SaleOrderForm(this, new DataModel.Model.SaleOrder
            {
                DateOriginated = DateTime.Now,
                SaleOrderNo = saleOrderNo,
                OriginUserId = userId
            });
            saleOrderForm.ShowDialog();
        }

        private void ButDeleteSaleOrder_Click(object sender, MouseButtonEventArgs e)
        {
            var saleOrder = GridSaleOrders.SelectedItem as DataModel.Model.SaleOrder;
            if (saleOrder != null)
            {
                if (saleOrder.SaleOrderStatus == (byte)DataType.SaleOrderStatus.Balanced)
                {
                    MessageBox.Show("已结算，不能删除！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {

                    _saleOrders.Remove(saleOrder);
                    _saleOrderRepository.Delete(saleOrder);
                    _unitOfWork.Commit();
                }
            }
        }

        private void ButEditSaleOrder_Click(object sender, MouseButtonEventArgs e)
        {
            var saleOrderForm = new SaleOrderForm(this, (DataModel.Model.SaleOrder)GridSaleOrders.SelectedItem);
            saleOrderForm.ShowDialog();
        }
    }


    public class SaleOrderDoMainModel
    {
        public DataModel.Model.SaleOrder SaleOrder { get; set; }
    }

    public class SOProduceDomainModel : INotifyPropertyChanged
    {
        public SOProduce SOProduce { get; set; }

        private float? _costPerUnit;
        public float? CostPerUnit
        {
            get { return _costPerUnit; }
            set
            {
                _costPerUnit = value;
                NotifyPropertyChange("CostPerUnit");
            }
        }

        private float _soProduceTotal;
        public float SOProduceTotal
        {
            get { return _soProduceTotal; }
            set
            {
                _soProduceTotal = value;
                NotifyPropertyChange("SOProduceTotal");
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
    }
}
