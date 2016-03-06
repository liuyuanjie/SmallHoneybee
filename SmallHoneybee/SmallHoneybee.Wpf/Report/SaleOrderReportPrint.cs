using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using SmallHoneybee.Common;
using SmallHoneybee.Wpf.Common;

namespace SmallHoneybee.Wpf.Report
{
    public class SaleOrderReportPrint
    {
        public static void Print(DataModel.Model.SaleOrder saleOrder)
        {
            using (
                ReportPrint report =
                    new ReportPrint(ResourcesHelper.SystemSettings[(short)DataType.SystemSettingCode.ReportPrintName]))
            {
                {
                    SaleOrderReportModel saleOrderReportModel = new SaleOrderReportModel
                    {
                        SOProucedDetailReportModels = new List<SOProucedDetailReportModel>(),
                        SOProuceSummaryReportModels = new List<SOProuceSummaryReportModel>
                        {
                            new SOProuceSummaryReportModel
                            {
                                MerchantsName =
                                    ResourcesHelper.SystemSettings[(short) DataType.SystemSettingCode.ReportMerchantsName],
                                OriginUser = ResourcesHelper.CurrentUser.Name,
                                SaleOrderNo = saleOrder.SaleOrderNo,
                                ProduceTotalCount = saleOrder.ProduceTotalCount ?? 0,
                                ProduceTotalOriginal = saleOrder.ProduceTotalOriginal ?? 0,
                                RealPrice = saleOrder.UserRealPrice ?? saleOrder.TotalCost ?? 0,
                                ReturnedPrice = saleOrder.UserReturnedPrice ?? 0,
                                ProduceTotalDiscountAndFavorableCost =
                                    (saleOrder.ProduceTotalDiscount ?? 0) + (saleOrder.FavorableCost ?? 0),
                                TotalCost = saleOrder.TotalCost ?? 0,
                                HowBalanceName =
                                    CommonHelper.Enumerate<DataType.SaleOrderBalancedMode>()
                                        .Single(x => x.Key == saleOrder.HowBalance)
                                        .Value,
                                Phone = ResourcesHelper.SystemSettings[(short) DataType.SystemSettingCode.ReportPhone],
                                Address =
                                    ResourcesHelper.SystemSettings[(short) DataType.SystemSettingCode.ReportAddress],
                                Healthline =
                                    ResourcesHelper.SystemSettings[(short) DataType.SystemSettingCode.ReportHealthline],
                                WebSiteUrl =
                                    ResourcesHelper.SystemSettings[(short) DataType.SystemSettingCode.ReportWebSiteUrl],

                            }
                        }
                    };
                    saleOrder.SOProduces.ForEach(x =>
                        saleOrderReportModel.SOProucedDetailReportModels.Add(new SOProucedDetailReportModel
                        {
                            ProduceName = x.Produce.Name,
                            CostPerUnit = x.CostPerUnit ?? 0,
                            Quantity = x.Quantity ?? 0,
                            RetailPrice = x.RetailPrice ?? 0
                        }));
                    report.Run(saleOrderReportModel);
                }
            }
        }
    }
}
