using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Impl;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;

namespace SmallHoneybee.Wpf.Views.Shared
{
    /// <summary>
    /// Interaction logic for FileDialog.xaml
    /// </summary>
    public partial class FileDialog : Window
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProduceRepository _produceRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly DataType.ImporterType _importerType;
        public FileDialog(DataType.ImporterType importerType)
        {
            InitializeComponent();
            _unitOfWork = UnityInit.UnitOfWork;
            _categoryRepository = _unitOfWork.GetRepository<CategoryRepository>();
            _produceRepository = _unitOfWork.GetRepository<ProduceRepository>();
            _purchaseOrderRepository = _unitOfWork.GetRepository<PurchaseOrderRepository>();

            _importerType = importerType;
        }

        private void btSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdFile = new OpenFileDialog
            {
                Filter = "(*.csv)|*.csv"
            };

            if (ofdFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(ofdFile.FileName))
                {
                    tbPath.Text = ofdFile.FileName;
                    btImport.IsEnabled = true;
                }
            }
        }

        private void btImport_Click(object sender, RoutedEventArgs e)
        {
            switch (_importerType)
            {
                case DataType.ImporterType.Produce:
                    ImportProduces();
                    break;

                case DataType.ImporterType.PurchaseOrder:
                    ImportPurchaseOrders();
                    break;
            }
        }

        private void ImportProduces()
        {
            List<DataModel.Model.Produce> produces = new List<DataModel.Model.Produce>();
            IEnumerable<Category> categories =
                _categoryRepository.Query().ToList();
            IEnumerable<IEnumerable<KeyValuePair<string, string>>> csvContengs = CsvHepler.GetCsvData(tbPath.Text);
            IEnumerable<KeyValuePair<byte, string>> procedureUnits = CommonHelper.Enumerate<DataType.ProduceUnit>();
            csvContengs.ForEach(x =>
            {
                DataModel.Model.Produce produce = new DataModel.Model.Produce();
                foreach (var y in x)
                {
                    if (!string.IsNullOrEmpty(y.Value))
                    {

                        DataType.ProduceField produceField =
                            (DataType.ProduceField)Enum.Parse(typeof(DataType.ProduceField), y.Key);
                        switch (produceField)
                        {
                            case DataType.ProduceField.Category:
                                Category category = categories
                                    .Single(c => y.Value.Equals(c.Name, StringComparison.CurrentCultureIgnoreCase));
                                produce.CategoryId = category != null ? category.CategoryId : categories.OrderBy(c => c.CategoryId).First().CategoryId;
                                continue;

                            case DataType.ProduceField.ProduceNo:
                                produce.ProduceNo = y.Value;
                                continue;

                            case DataType.ProduceField.Name:
                                produce.Name = y.Value;
                                continue;

                            case DataType.ProduceField.HowUsed:
                                KeyValuePair<byte, string>? howUsed = procedureUnits
                                    .SingleOrDefault(
                                        u => y.Value.Equals(u.Value, StringComparison.CurrentCultureIgnoreCase));
                                produce.HowUsed = (sbyte?)howUsed.Value.Key;
                                continue;

                            case DataType.ProduceField.FactoryPrice:
                                produce.FactoryPrice = float.Parse(y.Value);
                                continue;

                            case DataType.ProduceField.RetailPrice:
                                produce.RetailPrice = float.Parse(y.Value);
                                continue;

                            default:
                                continue;
                        }
                    }
                }
                produces.Add(produce);
            });

            var existedProduceNos = _produceRepository.Query().Select(x => x.ProduceNo);
            produces.RemoveAll(x => existedProduceNos.Contains(x.ProduceNo));
            _produceRepository.BatchInsertProduces(produces);
            _unitOfWork.Commit();

            System.Windows.MessageBox.Show("商品导入成功！", Properties.Resources.SystemName,
                MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void ImportPurchaseOrders()
        {
            DataModel.Model.PurchaseOrder purchaseOrder = _purchaseOrderRepository
                .Query()
                .OrderByDescending(x => x.DateOriginated)
                .FirstOrDefault(x=>x.POStatusCategory == (sbyte)DataType.POStatusCategory.Ordered);

            if (purchaseOrder == null)
            {
                return;
            }

            IList<DataModel.Model.Produce> produces = _produceRepository.Query().ToList();

            IEnumerable<IEnumerable<KeyValuePair<string, string>>> csvContents = CsvHepler.GetCsvData(tbPath.Text);
            IEnumerable<KeyValuePair<byte, string>> procedureUnits = CommonHelper.Enumerate<DataType.ProduceUnit>();

            csvContents.ForEach(x =>
            {
                DataModel.Model.POItem poItem = new DataModel.Model.POItem();
                foreach (var y in x)
                {
                    if (!string.IsNullOrEmpty(y.Value))
                    {
                        DataType.PurchaseOrderField purchaseOrderField =
                            (DataType.PurchaseOrderField)Enum.Parse(typeof(DataType.PurchaseOrderField), y.Key);
                        switch (purchaseOrderField)
                        {
                            case DataType.PurchaseOrderField.ProduceNo:
                                DataModel.Model.Produce produce = produces
                                    .Single(c => y.Value.Equals(c.ProduceNo, StringComparison.CurrentCultureIgnoreCase));
                                poItem.ProduceId = produce.ProduceId;
                                poItem.Produce = produce;
                                continue;

                            case DataType.PurchaseOrderField.PriceOrdered:
                                poItem.PriceOrdered = float.Parse(y.Value);
                                continue;

                            case DataType.PurchaseOrderField.PriceReceived:
                                poItem.PriceReceived = float.Parse(y.Value);
                                continue;

                            case DataType.PurchaseOrderField.QuantityOrdered:
                                poItem.QuantityOrdered = float.Parse(y.Value);
                                continue;

                            case DataType.PurchaseOrderField.QuantityReceived:
                                poItem.QuantityReceived = float.Parse(y.Value);
                                continue;

                            default:
                                continue;
                        }
                    }
                }
                poItem.POStatusCategory = (sbyte)DataType.POItemStatusCategory.Dispatched;
                poItem.CreatedBy = "admin";
                poItem.CreatedOn = DateTime.Now;
                poItem.LastModifiedBy = "admin";
                poItem.LastModifiedOn = DateTime.Now;
                poItem.RowVersion = DateTime.Now;
                poItem.Produce.Quantity += poItem.QuantityReceived ?? 0;
                poItem.Produce.Producelogs.Add(new Producelog
                {
                    ChangedBy = ResourcesHelper.CurrentUser.Name,
                    DateChanged = DateTime.Now,
                    NewValue = string.Format(ResourcesHelper.PurchaseOrderImporterFormat,
                        purchaseOrder.PurchaseOrderNo,
                        (poItem.QuantityReceived ?? 0).ToString("F2"),
                        (poItem.PriceReceived ?? 0).ToString("F2"),
                        poItem.Produce.Quantity.ToString("F2"))
                });
                poItem.Produce.LastOrderDate = DateTime.Now;

                purchaseOrder.POItems.Add(poItem);
            });
            purchaseOrder.POStatusCategory = (sbyte)DataType.POStatusCategory.Completed;
            purchaseOrder.TotalAmount = purchaseOrder.POItems.Sum(x => x.PriceReceived * x.QuantityReceived);
            purchaseOrder.GrandTotal = (purchaseOrder.TotalAmount ?? 0) + (purchaseOrder.TotalOther ?? 0)
                                       + (purchaseOrder.TotalShipping ?? 0) + (purchaseOrder.TotalTax ?? 0);
            _purchaseOrderRepository.Update(purchaseOrder);
            _unitOfWork.Commit();

            System.Windows.MessageBox.Show("订单导入成功！", Properties.Resources.SystemName,
                MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}
