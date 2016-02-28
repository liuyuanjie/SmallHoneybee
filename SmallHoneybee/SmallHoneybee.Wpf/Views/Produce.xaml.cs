﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
    /// Interaction logic for Produce1.xaml
    /// </summary>
    public partial class Produce : Page, INotifyPropertyChanged
    {
        private IUnitOfWork _unitOfWork;
        private ICategoryRepository _categoryRepository;
        private IProduceRepository _produceRepository;
        private ObservableCollection<DataModel.Model.Produce> _produces = new ObservableCollection<DataModel.Model.Produce>();

        private List<Category> _categories = new List<Category>();

        public Produce()
        {
            InitializeComponent();

            SetInitData();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<DataModel.Model.Produce> Produces
        {
            get { return _produces; }
            set
            {
                _produces = value;
                NotifyPropertyChange("Produces");
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            FileDialog fileDialog = new FileDialog(DataType.ImporterType.Produce);
            fileDialog.ShowDialog();
        }

        private void SetInitData()
        {
            var units = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            units.AddRange(CommonHelper.Enumerate<DataType.ProduceUnit>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            var enableTxets = new List<KeyValuePair<bool?, string>> { new KeyValuePair<bool?, string>(null, string.Empty) };
            enableTxets.AddRange(CommonHelper.EnableTexts.Select(x => new KeyValuePair<bool?, string>(x.Key, x.Value)));

            ExecuteSearchText();

            DataContext = new
            {
                Categories = _categories,
                Produces = _produces,
                Units = units,
                EnableTexts = enableTxets,
                ResourcesHelper.CurrentUserRolePermission,
            };

            if (!ResourcesHelper.CurrentUserRolePermission.ProduceFactoryPriceEdit)
            {
                DataGridColumn deletedColumn = null;
                gridProduces.Columns.ForEach(x =>
                {
                    if (x.Header.ToString() == "出厂价")
                    {
                        deletedColumn = x;
                    }
                });
                gridProduces.Columns.Remove(deletedColumn);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearSearchText();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            ExecuteSearchText();
        }

        private void ClearSearchText()
        {
            TxtSearchBox.Text = string.Empty;
        }

        private void ExecuteSearchText()
        {
            InitUnitOfWork();

            _categories.AddRange(_categoryRepository.Query().OrderBy(x => x.Name));

            _produces.Clear();
            _produceRepository
                .Query()
                .Where(x => x.BarCode.Contains(TxtSearchBox.Text) ||
                    x.ProduceNo.Contains(TxtSearchBox.Text) ||
                    x.Name.Contains(TxtSearchBox.Text))
                .ToList()
                .ForEach(x => _produces.Add(x));
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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _produces.Where(x => !string.IsNullOrEmpty(x.ProduceNo) &&
                    !string.IsNullOrEmpty(x.Name)).ForEach(x =>
                    {
                        x.BarCode = string.IsNullOrEmpty(x.BarCode) ? null : x.BarCode;
                        x.ProduceNo = string.IsNullOrEmpty(x.ProduceNo) ? null : x.ProduceNo;
                        //if (x.ProduceId > 0)
                        //{
                        //    //CommonHelper.UpdateModifiedOnAndDate(ResourcesHelper.CurrentUser, _produces);
                        //    _produceRepository.Update(x);
                        //}
                    });
                var duBarCodes = _produces.Where(x=>x.BarCode!=null).GroupBy(x => x.BarCode).Where(g => g.Count() > 1).Select(x => x.Key).ToList();
                var duProduceNos = _produces.Where(x => x.ProduceNo != null).GroupBy(x => x.ProduceNo).Where(g => g.Count() > 1).Select(x => x.Key).ToList();

                if (duBarCodes.Any() || duProduceNos.Any())
                {
                    var BarCodesMsg = duBarCodes.JoinStrings("|");
                    var duProduceNosMsg = duProduceNos.JoinStrings("|");
                    MessageBox.Show(string.Format("商品条形码或者商品编号不唯一！\r\n{0}\r\n{1}",
                            duBarCodes.Any() ? string.Format("商品条形码: {0}", BarCodesMsg) : string.Empty,
                            duProduceNos.Any() ? string.Format("商品编号不: {0}", duProduceNosMsg) : string.Empty),
                        SmallHoneybee.Wpf.Properties.Resources.SystemName,
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _unitOfWork.Commit();

                MessageBox.Show("保存成功！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                InitUnitOfWork();
                MessageBox.Show("保存失败！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                 MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                //DataGrid grd = (DataGrid)sender;
                //grd.BeginEdit(e);

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

        private void InitUnitOfWork()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _categoryRepository = _unitOfWork.GetRepository<CategoryRepository>();
            _produceRepository = _unitOfWork.GetRepository<ProduceRepository>();
        }
    }
}
