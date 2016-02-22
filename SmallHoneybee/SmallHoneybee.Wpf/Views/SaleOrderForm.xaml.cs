using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
    /// Interaction logic for SaleOrderForm.xaml
    /// </summary>
    public partial class SaleOrderForm : Window, INotifyPropertyChanged
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISaleOrderRepository _saleOrderRepository;
        private readonly ISOProduceRepository _soProduceRepository;
        private readonly IProduceRepository _produceRepository;

        public readonly SaleOrder SaleOrderWindow;
        private DataModel.Model.SaleOrder _saleOrder;
        private DataModel.Model.SaleOrder _oldSaleOrder;


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

            _unitOfWork = UnityInit.UnitOfWork;
            _saleOrderRepository = _unitOfWork.GetRepository<SaleOrderRepository>();
            _soProduceRepository = _unitOfWork.GetRepository<SOProduceRepository>();
            _produceRepository = _unitOfWork.GetRepository<ProduceRepository>();

            SaleOrderWindow = saleOrderWindow;
            _oldSaleOrder = saleOrder;

            if (saleOrder.SaleOrderId > 0)
            {
                _saleOrder = _saleOrderRepository.Query()
                    .Single(x => x.SaleOrderId == saleOrder.SaleOrderId);
                _saleOrder.SOProduces.ForEach(x => _soProduceDomainModels.Add(new SOProduceDomainModel
                {
                    SOProduce = x
                }));
            }
            else
            {
                _saleOrder = saleOrder;
            }

            DataContext = new
            {
                SaleOrder = _saleOrder,
                SOProduceDomainModels
            };

            _soProduceDomainModels.CollectionChanged += (sender, e) => SetTotalNameberText();

            SetTotalNameberText();
            if (_saleOrder.SaleOrderStatus == (byte)DataType.SaleOrderStatus.Balanced)
            {
                ButSaveAndBalance.IsEnabled = false;
                ButSave.IsEnabled = false;
            }
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

        private void SaveSaleOrder()
        {
            _soProduceDomainModels.ForEach(
                x => { x.SOProduce.CostPerUnit = x.SOProduce.DiscountRate * x.SOProduce.Produce.RetailPrice; });
            _saleOrder.ProduceCost = _soProduceDomainModels.Sum(x => x.SOProduce.Quantity * x.CostPerUnit);
            _saleOrder.TotalCost = (_saleOrder.OtherCost ?? 0) + _saleOrder.ProduceCost;
            _saleOrder.SaleOrderStatus = (sbyte)DataType.SaleOrderStatus.NotBalanced;

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

            _saleOrder.SaleOrderStatus = (sbyte)DataType.SaleOrderStatus.NotBalanced;
        }

        private void TxtBarCode_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var barCode = (TextBox)sender;
            if (barCode.IsFocused && !string.IsNullOrEmpty(barCode.Text) && barCode.Text.Length >= 3)
            {
                var produces = _produceRepository.Query().Where(x => x.BarCode.StartsWith(barCode.Text));
                if (produces.Count() == 1)
                {
                    var produce = produces.First();
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
                                CostPerUnit = produce.RetailPrice,
                                SaleOrder = _saleOrder,
                                SaleOrderId = _saleOrder.SaleOrderId,
                                DiscountRate = 1.00f
                            }
                        };
                        _soProduceDomainModels.Add(soProduceDomainModel);
                    }
                }
            }
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
                soProduceDomainModel.CostPerUnit
                    = soProduceDomainModel.SOProduce.CostPerUnit
                    = soProduceDomainModel.SOProduce.Produce.RetailPrice *
                        soProduceDomainModel.SOProduce.DiscountRate;
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
            TextTotalNumber.Text = string.Format("共 {0} 件，总计 {1}",
                    (_soProduceDomainModels.Where(x => x.SOProduce != null)
                        .Sum(x => x.SOProduce.Quantity) ?? 0).ToString("F2"),
                    (_saleOrder.TotalCost ?? 0).ToString("F2"));
        }

        private void ButSaveAndBalance_OnClick(object sender, RoutedEventArgs e)
        {
            SaveSaleOrder();
            var balanceFrom = new BalanceFrom(this, _unitOfWork, _saleOrder);
            balanceFrom.ShowDialog();
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
        }
    }
}
