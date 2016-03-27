using System;
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
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for DayBook.xaml
    /// </summary>
    public partial class DayBook : Page
    {
        private IUnitOfWork _unitOfWork;
        private EF.Data.Repository.IDayBookRepository _dayBookRepository;

        private ObservableCollection<DataModel.Model.DayBook> _dayBooks = new ObservableCollection<DataModel.Model.DayBook>();

        public ObservableCollection<DataModel.Model.DayBook> DayBooks
        {
            get { return _dayBooks; }
            set
            {
                _dayBooks = value;
                NotifyPropertyChange("DayBooks");
            }
        }

        public DayBook()
        {
            InitializeComponent();

            SetInitData();
        }

        private void SetInitData()
        {
            var dayBookTypes = new List<KeyValuePair<sbyte, string>>();
            dayBookTypes.AddRange(CommonHelper.Enumerate<DataType.DayBookType>().Select(x => new KeyValuePair<sbyte, string>((sbyte)x.Key, x.Value)));

            DateStartDate.SelectedDate = DateTime.Today.AddDays(-(byte)DateTime.Today.DayOfWeek);
            DateEndDate.SelectedDate = DateTime.Today.AddDays(7 - (byte)DateTime.Today.DayOfWeek - 1);
            ExecuteSearchText();

            DataContext = new
            {
                DayBookTypes = dayBookTypes,
                DayBooks,
            };
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
            DateStartDate.SelectedDate = DateTime.Today.AddDays(-(byte)DateTime.Today.DayOfWeek);
            DateEndDate.SelectedDate = DateTime.Today.AddDays(7 - (byte)DateTime.Today.DayOfWeek - 1);
        }

        private void ExecuteSearchText()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _dayBookRepository = _unitOfWork.GetRepository<DayBookRepository>();

            _dayBooks.Clear();

            IQueryable<DataModel.Model.DayBook> dayBooks = _dayBookRepository.Query();

            DateTime? startDate = DateStartDate.SelectedDate;
            DateTime? endDate = DateEndDate.SelectedDate;

            if (DateStartDate.SelectedDate.HasValue && DateEndDate.SelectedDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);
                dayBooks = dayBooks.Where(x => x.DayBookDate >= startDate &&
                    x.DayBookDate < endDate);
            }
            else if (DateStartDate.SelectedDate.HasValue)
            {
                dayBooks = dayBooks.Where(x => x.DayBookDate >= startDate);
            }
            else if (DateEndDate.SelectedDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);

                dayBooks = dayBooks.Where(x => x.DayBookDate < endDate);
            }

            dayBooks
                .OrderByDescending(x => x.DayBookDate)
                .ToList()
                .ForEach(x => _dayBooks.Add(x));

            InitBlankRow();
        }

        private void CommandBinding_ClearSearchText_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void CommandBinding_ExecuteSearchText_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void CommandBinding_ClearSearchText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClearSearchText();
        }

        private void CommandBinding_ExecuteSearchText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExecuteSearchText();
        }

        private void ButDeleteDayBook_Click(object sender, RoutedEventArgs e)
        {
            var dayBook = gridDayBooks.SelectedItem as DataModel.Model.DayBook;
            if (dayBook != null)
            {
                _dayBooks.Remove(dayBook);
                if (dayBook.DayBookId > 0)
                {
                    _dayBookRepository.Delete(dayBook);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dayBooks.Where(x => x.DayBookDate >= DateTime.Today && x.HowManey >= 0).ForEach(x =>
                {
                    if (x.DayBookId > 0)
                    {
                        CommonHelper.UpdateModifiedOnAndDate(ResourcesHelper.CurrentUser, x);
                        _dayBookRepository.Update(x);
                    }
                    else
                    {
                        _dayBookRepository.Create(x);
                    }
                });

                _unitOfWork.Commit();

                MessageBox.Show("保存成功！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log4NetHelper.WriteLog(ex.ToString());

                MessageBox.Show("保存失败！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                 MessageBoxButton.OK, MessageBoxImage.Error);
                ExecuteSearchText();
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

        private void TextSettlementMen_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                InitBlankRow();
            }
        }

        private void InitBlankRow()
        {
            for (int i = 0; i < 5; i++)
            {
                _dayBooks.Add(new DataModel.Model.DayBook
                {
                    DayBookType = 1,
                    DayBookDate = DateTime.Now,
                    CreatedBy = ResourcesHelper.CurrentUser.Name,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = ResourcesHelper.CurrentUser.Name,
                    LastModifiedOn = DateTime.Now,
                });
            }
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
}
