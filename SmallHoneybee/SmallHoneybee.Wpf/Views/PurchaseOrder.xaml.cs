using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using SmallHoneybee.Wpf.Views.Shared;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for PurchaseOrder.xaml
    /// </summary>
    public partial class PurchaseOrder : Page
    {
        private IUnitOfWork _unitOfWork;
        private ICategoryRepository _categoryRepository;
        private IPurchaseOrderRepository _purchaseOrderRepository;
        private IProduceRepository _produceRepository;

        private ObservableCollection<DataModel.Model.PurchaseOrder> _purchaseOrders
            = new ObservableCollection<DataModel.Model.PurchaseOrder>();
        private ObservableCollection<POItemDomainModel> _poItemDomainModels
            = new ObservableCollection<POItemDomainModel>();
        public PurchaseOrder()
        {
            InitializeComponent();

            SetInitData();
        }


        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new FileDialog(DataType.ImporterType.PurchaseOrder);
            fileDialog.ShowDialog();
        }

        private void SetInitData()
        {
            InitUnitOfWork();

            var categories = new List<Category>();
            categories.AddRange(_categoryRepository.Query().OrderBy(x => x.Name));

            var poStatusCategories = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            poStatusCategories.AddRange(CommonHelper.Enumerate<DataType.POStatusCategory>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            var poItemStatusCategories = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            poItemStatusCategories.AddRange(CommonHelper.Enumerate<DataType.POItemStatusCategory>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            ExecuteSearchText();

            DataContext = new
            {
                Categories = categories,
                PurchaseOrders = _purchaseOrders,
                POItemDomainModels = _poItemDomainModels,
                POItemStatusCategories = poItemStatusCategories,
                POStatusCategories = poStatusCategories,
            };
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearSearchText();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSearchText();
            if (GridPurchaseOrders.Items.Count > 0)
            {
                GridPurchaseOrders.SelectedIndex = 0;
            }
        }

        private void ClearSearchText()
        {
            TxtSearchBox.Text = string.Empty;
        }

        private void ExecuteSearchText()
        {
            InitUnitOfWork();

            _purchaseOrders.Clear();
            _purchaseOrderRepository
                .Query()
                .Where(x => x.POContractNo.Contains(TxtSearchBox.Text) ||
                    x.PurchaseOrderNo.Contains(TxtSearchBox.Text) ||
                    x.Name.Contains(TxtSearchBox.Text))
                .OrderByDescending(x => x.DateCompleted)
                .ToList()
                .ForEach(x => _purchaseOrders.Add(x));

            _purchaseOrders.Add(new DataModel.Model.PurchaseOrder
            {
                DateCompleted = DateTime.Now,
                CreatedBy = ResourcesHelper.CurrentUser.Name,
                CreatedOn = DateTime.Now,
                LastModifiedBy = ResourcesHelper.CurrentUser.Name,
                LastModifiedOn = DateTime.Now,
                OriginatorId = ResourcesHelper.CurrentUser.UserId
            });

            TxtTotalInfo.Text = string.Format("共{0}笔订单, 合计{1}金额", _purchaseOrders.Count, _purchaseOrders.Sum(x => x.GrandTotal));
        }

        private void InitUnitOfWork()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _categoryRepository = _unitOfWork.GetRepository<CategoryRepository>();
            _purchaseOrderRepository = _unitOfWork.GetRepository<PurchaseOrderRepository>();
            _produceRepository = _unitOfWork.GetRepository<ProduceRepository>();
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

        private void GridPurchaseOrders_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _poItemDomainModels.Clear();

            DataModel.Model.PurchaseOrder purchaseOrder = (DataModel.Model.PurchaseOrder)((DataGrid)sender).CurrentItem;
            if (purchaseOrder == null && ((DataGrid)sender).Items.Count > 0)
            {
                purchaseOrder = (DataModel.Model.PurchaseOrder)((DataGrid)sender).Items[0];
            }

            if (purchaseOrder != null)
            {
                purchaseOrder.POItems.ForEach(x => _poItemDomainModels.Add(new POItemDomainModel
                {
                    POItem = x
                }));

                if (purchaseOrder.POStatusCategory != (sbyte)DataType.POStatusCategory.Completed)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        _poItemDomainModels.Add(new POItemDomainModel
                        {
                            POItem = new POItem
                            {
                                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                                CreatedBy = ResourcesHelper.CurrentUser.Name,
                                CreatedOn = DateTime.Now,
                                LastModifiedBy = ResourcesHelper.CurrentUser.Name,
                                LastModifiedOn = DateTime.Now,
                            }
                        });
                    }
                }

                TxtDetailTotalInfo.Text = string.Format("共{0}种商品, 合计{1}件, 合计金额: {2}",
                    purchaseOrder.POItems.Count.ToString("F2"),
                    purchaseOrder.POItems.Sum(x => x.QuantityReceived ?? 0).ToString("F2"),
                    (purchaseOrder.POItems.Sum(x => x.QuantityReceived * x.PriceReceived) ?? 0).ToString("F2"));
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataModel.Model.PurchaseOrder purchaseOrder =
                (DataModel.Model.PurchaseOrder)GridPurchaseOrders.SelectedItem;
                if (purchaseOrder != null)
                {
                    purchaseOrder.POContractNo = purchaseOrder.PurchaseOrderNo;
                    purchaseOrder.POItems = _poItemDomainModels
                        .Where(x => x.POItem.Produce != null &&
                            x.POItem.POStatusCategory != (sbyte)DataType.POStatusCategory.Completed)
                        .Select(x => x.POItem).ToList();
                    if (purchaseOrder.POItems.Count == 0)
                    {
                        MessageBox.Show("没有订单条目需要保存！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    purchaseOrder.POItems.ForEach(x =>
                    {
                        x.CreatedBy = ResourcesHelper.CurrentUser.Name;
                        x.CreatedOn = DateTime.Now;
                        x.LastModifiedBy = ResourcesHelper.CurrentUser.Name;
                        x.LastModifiedOn = DateTime.Now;
                        if (purchaseOrder.POStatusCategory == (sbyte)DataType.POStatusCategory.Completed)
                        {
                            x.Produce.Quantity += x.QuantityReceived ?? 0;
                            x.Produce.Producelogs.Add(new Producelog
                            {
                                ChangedBy = ResourcesHelper.CurrentUser.Name,
                                DateChanged = DateTime.Now,
                                NewValue = string.Format(ResourcesHelper.PurchaseOrderImporterFormat,
                                    purchaseOrder.PurchaseOrderNo,
                                    (x.QuantityReceived ?? 0).ToString("F2"),
                                    (x.PriceReceived ?? 0).ToString("F2"),
                                    x.Produce.Quantity.ToString("F2"))
                            });
                            x.Produce.LastOrderDate = DateTime.Now;
                            x.POStatusCategory = (sbyte)DataType.POStatusCategory.Completed;
                        }
                    });

                    purchaseOrder.TotalAmount = purchaseOrder.POItems.Sum(x => x.PriceReceived * x.QuantityReceived);
                    purchaseOrder.GrandTotal = (purchaseOrder.TotalAmount ?? 0) + (purchaseOrder.TotalOther ?? 0)
                                               + (purchaseOrder.TotalShipping ?? 0) + (purchaseOrder.TotalTax ?? 0);

                    if (purchaseOrder.PurchaseOrderId == 0)
                    {
                        _purchaseOrderRepository.Create(purchaseOrder);
                    }
                    else
                    {
                        _purchaseOrderRepository.Update(purchaseOrder);
                    }

                    _unitOfWork.Commit();

                    MessageBox.Show("保存成功！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    ExecuteSearchText();
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.WriteLog(ex.ToString());

                MessageBox.Show("保存失败！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                 MessageBoxButton.OK, MessageBoxImage.Error);
                ExecuteSearchText();
            }

        }

        private void TxtBarCode_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var barCode = (TextBox)sender;
                if (barCode.IsFocused)
                {
                    var produce = _produceRepository.Query().FirstOrDefault(x => x.BarCode.StartsWith(barCode.Text));
                    if (produce != null)
                    {
                        var poItemDomainModel = GridPOItems.SelectedItem as POItemDomainModel;
                        //if (poItemDomainModel == null)
                        //{
                        //    poItemDomainModel = new POItemDomainModel
                        //    {
                        //        POItem = new POItem
                        //        {
                        //            Produce = produce,
                        //            ProduceId = produce.ProduceId,
                        //        }
                        //    };
                        //    _poItemDomainModels.Add(poItemDomainModel);

                        //}
                        //else
                        //{
                        //    poItemDomainModel.POItem.Produce = produce;
                        //    poItemDomainModel.POItem.ProduceId = produce.ProduceId;
                        //}
                        poItemDomainModel = new POItemDomainModel
                        {
                            POItem = new POItem
                            {
                                Produce = produce,
                                ProduceId = produce.ProduceId,
                            }
                        };
                        _poItemDomainModels.Insert(_poItemDomainModels.Count(x => x.POItem.ProduceId > 0), poItemDomainModel);
                        barCode.Text = string.Empty;
                    }
                }
            }
        }

        private void ButDeletePOItem_Click(object sender, RoutedEventArgs e)
        {
            var poItemDomainModel = GridPOItems.SelectedItem as POItemDomainModel;
            if (poItemDomainModel != null)
            {
                if (poItemDomainModel.POItem.POStatusCategory == (sbyte)DataType.POStatusCategory.Completed)
                {
                    MessageBox.Show("订单已分配, 无法删除！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _poItemDomainModels.Remove(poItemDomainModel);
            }
        }
    }

    public class POItemDomainModel
    {
        public POItem POItem { get; set; }

        public float POItemTotal
        {
            get
            {
                if (POItem.POStatusCategory == (byte)DataType.POItemStatusCategory.Dispatched)
                {
                    return (POItem.QuantityReceived ?? 0) * (POItem.PriceReceived ?? 0);

                }
                return (POItem.QuantityOrdered ?? 0) * (POItem.PriceOrdered ?? 0);
            }
        }
    }
}
