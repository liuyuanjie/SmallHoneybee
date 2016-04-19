using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Practices.ObjectBuilder2;
using SmallHoneybee.Common;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class MemberCardDetail : Page, INotifyPropertyChanged
    {
        private IUnitOfWork _unitOfWork;
        private IMemberCardRepository _memberCardRepository;
        private IMemberCardLogRepository _memberCardLogRepository;
        private IUserRepository _userRepository;

        private ObservableCollection<MemberCardDetailModel> _memberCardDetails = new ObservableCollection<MemberCardDetailModel>();

        public ObservableCollection<MemberCardDetailModel> MemberCardDetails
        {
            get { return _memberCardDetails; }
            set
            {
                _memberCardDetails = value;
                NotifyPropertyChange("MemberCardDetails");
            }
        }

        public MemberCardDetail()
        {
            InitializeComponent();

            SetInitData();
        }

        private void SetInitData()
        {
            DateStartDate.SelectedDate = DateTime.Today.AddDays(-(byte)DateTime.Today.DayOfWeek);
            DateEndDate.SelectedDate = DateTime.Today.AddDays(7 - (byte)DateTime.Today.DayOfWeek - 1);

            var logTypeTexts = new List<KeyValuePair<sbyte, string>>();
            logTypeTexts.AddRange(CommonHelper.Enumerate<DataType.MemberCardLogType>().Select(x => new KeyValuePair<sbyte, string>((sbyte)x.Key, x.Value)));

            var howBalances = new List<KeyValuePair<sbyte?, string>> { new KeyValuePair<sbyte?, string>(null, string.Empty) };
            howBalances.AddRange(CommonHelper.Enumerate<DataType.SaleOrderBalancedMode>().Select(x => new KeyValuePair<sbyte?, string>((sbyte)x.Key, x.Value)));

            ExecuteSearchText();

            DataContext = new
            {
                LogTypeTexts = logTypeTexts,
                MemberCardDetailModel = _memberCardDetails,
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
        }

        private void ClearSearchText()
        {
            TxtSearchBox.Text = string.Empty;
        }

        private void ExecuteSearchText()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _memberCardRepository = _unitOfWork.GetRepository<MemberCardRepository>();
            _userRepository = _unitOfWork.GetRepository<UserRepository>();
            _memberCardLogRepository = _unitOfWork.GetRepository<MemberCardLogRepository>();


            DateTime? startDate = DateStartDate.SelectedDate;
            DateTime? endDate = DateEndDate.SelectedDate;

            if (startDate.HasValue && endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);
            }
            else if (startDate.HasValue)
            {
                endDate = DateTime.MaxValue;
            }
            else if (endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1);
                startDate = DateTime.MinValue;
            }

            _memberCardDetails.Clear();
            var memberCardDetails =
                from user in _userRepository
                    .Query()
                    .Where(x => x.Name.Contains(TxtSearchBox.Text) ||
                                x.Phone.Contains(TxtSearchBox.Text))
                join memberCard in _memberCardRepository
                    .Query()
                    .Where(x => x.MemberCardNo.Contains(TxtSearchBox.Text))
                    on user.UserId equals memberCard.RelateUserId
                join memberCardLog in _memberCardLogRepository
                    .Query()
                    .Where(x => x.DateChanged >= startDate &&
                                x.DateChanged < endDate)
                    on memberCard.MemberCardId equals memberCardLog.MemberCardId
                select new MemberCardDetailModel
                {
                    MemberCardId = memberCard.MemberCardId,
                    DateChanged = memberCardLog.DateChanged,
                    ChangedBy = memberCardLog.ChangedBy,
                    MemberCardNo = memberCard.MemberCardNo,
                    RelateUser = user.Name,
                    PrincipalMoney = memberCardLog.PrincipalMoney,
                    FavorableMoney = memberCardLog.FavorableMoney,
                    TotalMoney = memberCardLog.PrincipalMoney + memberCardLog.FavorableMoney,
                    Description = memberCardLog.NewValue,
                    LogType = memberCardLog.LogType,
                };

            if (ComboTransactionType.SelectedValue != null)
            {
                sbyte transactionType = (sbyte)ComboTransactionType.SelectedValue;
                memberCardDetails = memberCardDetails.Where(x => x.LogType == transactionType);
            }

            if (ComboHowBalance.SelectedValue != null)
            {
                memberCardDetails = memberCardDetails.Where(x => x.Description.Contains(ComboHowBalance.Text));
            }

            memberCardDetails
                .OrderByDescending(x => x.DateChanged)
                .ToList()
                .ForEach(x => _memberCardDetails.Add(x));

            TxtTotalInfo.Text = string.Format("共计 {0} 条记录, 交易本金: {1}, 交易赠送金额: {2}，交易总金额: {3}",
                memberCardDetails.Count(),
                (memberCardDetails.Sum(x => (float?)x.PrincipalMoney) ?? 0).ToString("F2"),
                (memberCardDetails.Sum(x => (float?)x.FavorableMoney) ?? 0).ToString("F2"),
                (memberCardDetails.Sum(x => (float?)x.FavorableMoney + (float?)x.PrincipalMoney) ?? 0).ToString("F2"));
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


        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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

    public class MemberCardDetailModel
    {
        public int MemberCardId { get; set; }
        public DateTime DateChanged { get; set; }
        public string ChangedBy { get; set; }
        public string MemberCardNo { get; set; }
        public string RelateUser { get; set; }
        public float TotalMoney { get; set; }
        public float PrincipalMoney { get; set; }
        public float FavorableMoney { get; set; }
        public string Description { get; set; }
        public sbyte? LogType { get; set; }


    }
}
