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
            _unitOfWork = UnityInit.UnitOfWork;
            _categoryRepository = _unitOfWork.GetRepository<CategoryRepository>();
            _purchaseOrderRepository = _unitOfWork.GetRepository<PurchaseOrderRepository>();

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
            _purchaseOrders.Clear();
            _purchaseOrderRepository
                .Query()
                .Where(x => x.POContractNo.Contains(TxtSearchBox.Text) ||
                    x.PurchaseOrderNo.Contains(TxtSearchBox.Text) ||
                    x.Name.Contains(TxtSearchBox.Text))
                .ToList()
                .ForEach(x => _purchaseOrders.Add(x));
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
            if (purchaseOrder != null)
            {
                purchaseOrder.POItems.ForEach(x => _poItemDomainModels.Add(new POItemDomainModel
                {
                    POItem = x
                }));
            }
            else
            {
                if (((DataGrid)sender).Items.Count > 0)
                {
                    purchaseOrder = (DataModel.Model.PurchaseOrder)((DataGrid)sender).Items[0];
                    purchaseOrder.POItems.ForEach(x => _poItemDomainModels.Add(new POItemDomainModel
                    {
                        POItem = x
                    }));
                }
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
