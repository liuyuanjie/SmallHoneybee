using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ExecuteSearchText();

            var saleOrderStatuses = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            saleOrderStatuses.AddRange(CommonHelper.Enumerate<DataType.SaleOrderStatus>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            DataContext = new
            {
                SaleOrders = _saleOrders,
                SOProduceDomainModels = _soProduceDomainModels,
                SaleOrderStatusTexts = saleOrderStatuses
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
            _saleOrderRepository
                .Query()
                .OrderByDescending(x => x.DateOriginated)
                .Where(x => x.SaleOrderNo.Contains(TxtSearchBox.Text) ||
                    x.Name.Contains(TxtSearchBox.Text))
                .ToList()
                .ForEach(x => _saleOrders.Add(x));
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
                SaleOrderNo = saleOrderNo
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
        public float? CostPerUnit
        {
            get { return SOProduce.CostPerUnit; }
            set
            {
                NotifyPropertyChange("CostPerUnit");
            }
        }

        public float SOProduceTotal
        {
            get { return (SOProduce.Quantity ?? 0) * (SOProduce.CostPerUnit ?? 0); }
            set
            {
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
