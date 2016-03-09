using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using SmallHoneybee.Common;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;
using SmallHoneybee.EF.Data.Repository.Impl;
using SmallHoneybee.EF.Data.UntityContainer;

namespace SmallHoneybee.Wpf.Views
{
    /// <summary>
    /// Interaction logic for SystemSetting.xaml
    /// </summary>
    public partial class SystemSetting : Page
    {
        private IUnitOfWork _unitOfWork;
        private ISystemSettingRepository _systemSettingRepository;
        private SystemSettingModel systemSetting = new SystemSettingModel();

        IList<DataModel.Model.SystemSetting> setting;
        public SystemSetting()
        {
            InitializeComponent();

            SetSystemSetting();
            DataContext = new
            {
                SystemSetting = systemSetting
            };
        }

        private void ButSave_OnClick(object sender, RoutedEventArgs e)
        {
            var setting = _systemSettingRepository.Query().Where(x => x.IsEnable).ToList();
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.MemberPointsRate).ToString()).SettingValue = systemSetting.MemberPointsRate;
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.SOProduceGeneralMangerMaxDiscountPrice).ToString()).SettingValue = systemSetting.SOProduceGeneralMangerMaxDiscountPrice;
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportMerchantsName).ToString()).SettingValue = systemSetting.ReportMerchantsName;
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportPhone).ToString()).SettingValue = systemSetting.ReportPhone;
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportAddress).ToString()).SettingValue = systemSetting.ReportAddress;
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportPrintName).ToString()).SettingValue = systemSetting.ReportPrintName;
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportHealthline).ToString()).SettingValue = systemSetting.ReportHealthline;
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportWebSiteUrl).ToString()).SettingValue = systemSetting.ReportWebSiteUrl;
            setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.DefaultMemberCardPW).ToString()).SettingValue = systemSetting.DefaultMemberCardPW;
            _unitOfWork.Commit();
        }

        private void ButCancel_OnClick(object sender, RoutedEventArgs e)
        {
            SetSystemSetting();
        }

        private void SetSystemSetting()
        {
            _unitOfWork = UnityInit.UnitOfWork;
            _systemSettingRepository = _unitOfWork.GetRepository<SystemSettingRepository>();

            setting = _systemSettingRepository.Query().Where(x => x.IsEnable).ToList();

            systemSetting.MemberPointsRate =
                setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.MemberPointsRate).ToString()).SettingValue;
            systemSetting.SOProduceGeneralMangerMaxDiscountPrice = setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.SOProduceGeneralMangerMaxDiscountPrice).ToString()).SettingValue;
            systemSetting.ReportMerchantsName = setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportMerchantsName).ToString()).SettingValue;
            systemSetting.ReportPhone = setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportPhone).ToString()).SettingValue;
            systemSetting.ReportAddress = setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportAddress).ToString()).SettingValue;
            systemSetting.ReportPrintName = setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportPrintName).ToString()).SettingValue;
            systemSetting.ReportHealthline = setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportHealthline).ToString()).SettingValue;
            systemSetting.ReportWebSiteUrl = setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.ReportWebSiteUrl).ToString()).SettingValue;
            systemSetting.DefaultMemberCardPW = setting.Single(x => x.SettingCode == ((short)DataType.SystemSettingCode.DefaultMemberCardPW).ToString()).SettingValue;
        }
    }

    public class SystemSettingModel
    {
        public string SystemVersion { get; set; }
        public string MemberPointsRate { get; set; }
        public string SOProduceGeneralMangerMaxDiscountPrice { get; set; }
        public string ReportMerchantsName { get; set; }
        public string ReportPhone { get; set; }
        public string ReportAddress { get; set; }
        public string ReportPrintName { get; set; }
        public string ReportHealthline { get; set; }
        public string ReportWebSiteUrl { get; set; }
        public string DefaultMemberCardPW { get; set; }
    }
}
