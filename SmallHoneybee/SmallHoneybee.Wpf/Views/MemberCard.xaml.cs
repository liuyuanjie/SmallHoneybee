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
    public partial class MemberCard : Page, INotifyPropertyChanged
    {
        private IUnitOfWork _unitOfWork;
        private IMemberCardRepository _memberCardRepository;

        private ObservableCollection<MemberCardModel> _memberCards = new ObservableCollection<MemberCardModel>();

        public ObservableCollection<MemberCardModel> MemberCards
        {
            get { return _memberCards; }
            set
            {
                _memberCards = value;
                NotifyPropertyChange("MemberCards");
            }
        }

        public MemberCard()
        {
            InitializeComponent();

            SetInitData();
        }

        private void SetInitData()
        {
            var enableTxets = new List<KeyValuePair<bool?, string>> { new KeyValuePair<bool?, string>(null, string.Empty) };
            enableTxets.AddRange(CommonHelper.EnableTexts.Select(x => new KeyValuePair<bool?, string>(x.Key, x.Value)));

            var memberTypes = new List<KeyValuePair<sbyte, string>>();
            memberTypes.AddRange(CommonHelper.Enumerate<DataType.MemberType>().Select(x => new KeyValuePair<sbyte, string>((sbyte)x.Key, x.Value)));

            var memberCardStatuses = new List<KeyValuePair<sbyte, string>>();
            memberCardStatuses.AddRange(CommonHelper.Enumerate<DataType.MemberCardStatus>().Select(x => new KeyValuePair<sbyte, string>((sbyte)x.Key, x.Value)));

            ExecuteSearchText();

            DataContext = new
            {
                MemberTypes = memberTypes,
                MemberCards = _memberCards,
                EnableTexts = enableTxets,
                MemberCardStatuses = memberCardStatuses,
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

            _memberCards.Clear();
            _memberCardRepository
                .Query()
                .Where(x =>
                    x.MemberCardNo.Contains(TxtSearchBox.Text) ||
                    x.Name.Contains(TxtSearchBox.Text))
                .ToList()
                .ForEach(x => _memberCards.Add(new MemberCardModel
                {
                    MemberCard = x,
                    RelateUserName = x.RelateUserId.HasValue
                        ? x.RelateUserUser.Name
                        : string.Empty,

                    DispatchUserName = x.DispatchUserId.HasValue
                        ? x.DispatchUserUser.Name
                        : string.Empty,
                    CanUpdate = x.MemberCardStatus == (sbyte)DataType.MemberCardStatus.NonActive &&
                        !x.RelateUserId.HasValue
                }));

            InitBlankRow();
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

        private void ButDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var memberCard = gridMemberCards.SelectedItem as MemberCardModel;
            if (memberCard != null)
            {
                _memberCards.Remove(memberCard);
                if (memberCard.MemberCard.MemberCardId > 0)
                {
                    _memberCardRepository.Delete(memberCard.MemberCard);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _memberCards.Where(x => !string.IsNullOrEmpty(x.MemberCard.Name) &&
                    !string.IsNullOrEmpty(x.MemberCard.MemberCardNo)).ForEach(x =>
                {
                    if (x.MemberCard.MemberCardId > 0)
                    {
                        if (!x.MemberCard.RelateUserId.HasValue)
                        {
                            x.MemberCard.TotalSurplusMoney = x.MemberCard.MemberMoney;
                            x.MemberCard.PrincipalSurplusMoney = x.MemberCard.MemberMoney;
                        }
                        CommonHelper.UpdateModifiedOnAndDate(ResourcesHelper.CurrentUser, _memberCards);
                        _memberCardRepository.Update(x.MemberCard);
                    }
                    else
                    {
                        x.MemberCard.TotalSurplusMoney = x.MemberCard.MemberMoney;
                        x.MemberCard.PrincipalSurplusMoney = x.MemberCard.MemberMoney;
                        _memberCardRepository.Create(x.MemberCard);
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

        private void TextName_OnKeyDown(object sender, KeyEventArgs e)
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
                _memberCards.Add(
                    new MemberCardModel
                    {
                        MemberCard = new DataModel.Model.MemberCard
                        {
                            MemberType = 0,
                            IsEnable = true,
                            PasswordHash =
                                SaltedHash.Create(
                                    ResourcesHelper.SystemSettings[
                                        (short)DataType.SystemSettingCode.DefaultMemberCardPW]).Hash,
                            PasswordSalt =
                                SaltedHash.Create(
                                    ResourcesHelper.SystemSettings[
                                        (short)DataType.SystemSettingCode.DefaultMemberCardPW]).Salt,
                            MemberMoney = 0,
                            TotalSurplusMoney = 0,
                            PrincipalSurplusMoney = 0,
                            FavorableSurplusMoney = 0,
                            DispatchDate = DateTime.Now,
                            CreatedBy = ResourcesHelper.CurrentUser.Name,
                            CreatedOn = DateTime.Now,
                            LastModifiedBy = ResourcesHelper.CurrentUser.Name,
                            LastModifiedOn = DateTime.Now,
                            MemberCardNo = ResourcesHelper.MFTMemberCard
                        },
                        CanUpdate = true
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

        private void ButShowLog_Click(object sender, RoutedEventArgs e)
        {
            var memberCard = gridMemberCards.SelectedItem as MemberCardModel;
            if (memberCard != null)
            {
                SmallHoneybee.Wpf.Views.MemberCardLog memberCardLog = new SmallHoneybee.Wpf.Views.MemberCardLog(memberCard.MemberCard.MemberCardId);
                memberCardLog.ShowDialog();
            }
        }
    }

    public class MemberCardModel
    {
        public DataModel.Model.MemberCard MemberCard { get; set; }
        public string RelateUserName { get; set; }
        public string DispatchUserName { get; set; }
        public bool CanUpdate { get; set; }

    }
}
