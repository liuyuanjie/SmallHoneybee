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
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;
using SmallHoneybee.Wpf.Common;
using SmallHoneybee.Wpf.Views.Shared;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class MemberCard : Page, INotifyPropertyChanged
    {
        private IUnitOfWork _unitOfWork;
        private IMemberCardRepository _memberCardRepository;

        private ObservableCollection<DataModel.Model.MemberCard> _memberCards = new ObservableCollection<DataModel.Model.MemberCard>();

        public ObservableCollection<DataModel.Model.MemberCard> MemberCards
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
                .ForEach(x => _memberCards.Add(x));

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

        private void ButDeleteUser_Click(object sender, MouseButtonEventArgs e)
        {
            var memberCard = gridUsers.SelectedItem as DataModel.Model.MemberCard;
            if (memberCard != null)
            {
                _memberCards.Remove(memberCard);
                if (memberCard.MemberCardId > 0)
                {
                    _memberCardRepository.Delete(memberCard);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _memberCards.Where(x => !string.IsNullOrEmpty(x.Name) &&
                    !string.IsNullOrEmpty(x.MemberCardNo)).ForEach(x =>
                {
                    if (x.MemberCardId > 0)
                    {
                        CommonHelper.UpdateModifiedOnAndDate(ResourcesHelper.CurrentUser, _memberCards);
                        _memberCardRepository.Update(x);
                    }
                    else
                    {
                        _memberCardRepository.Create(x);
                    }
                });

                _unitOfWork.Commit();

                MessageBox.Show("保存成功！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("保存失败！", SmallHoneybee.Wpf.Properties.Resources.SystemName,
                 MessageBoxButton.OK, MessageBoxImage.Error);
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
            _memberCards.Add(new DataModel.Model.MemberCard
            {
                MemberType = 0,
                IsEnable = true,
                PasswordHash = SaltedHash.Create(ResourcesHelper.SystemSettings[(short)DataType.SystemSettingCode.DefaultMemberCardPW]).Hash,
                PasswordSalt = SaltedHash.Create(ResourcesHelper.SystemSettings[(short)DataType.SystemSettingCode.DefaultMemberCardPW]).Salt,
                MemberMoney = 0,
                SurplusMoney = 0,
                DispatchDate=DateTime.Now,
                CreatedBy = ResourcesHelper.CurrentUser.Name,
                CreatedOn = DateTime.Now,
                LastModifiedBy = ResourcesHelper.CurrentUser.Name,
                LastModifiedOn = DateTime.Now,
            });
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
