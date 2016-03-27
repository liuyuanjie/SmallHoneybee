using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.Wpf.Report
{
    public class SaleOrderReportModel
    {
        public IList<SOProucedDetailReportModel> SOProucedDetailReportModels { get; set; }
        public IList<SOProuceSummaryReportModel> SOProuceSummaryReportModels { get; set; }
    }

    public class SOProucedDetailReportModel
    {
        public string ProduceName { get; set; }
        public float Quantity { get; set; }
        public float RetailPrice { get; set; }
        public float CostPerUnit { get; set; }

        public float SOProduceTotal
        {
            get
            {
                return CostPerUnit * Quantity;
            }
        }
    }

    public class SOProuceSummaryReportModel
    {
        public string MerchantsName { get; set; }
        public string OriginUser { get; set; }
        public string SaleOrderNo { get; set; }
        public float ProduceTotalCount { get; set; }
        public float ProduceTotalOriginal { get; set; }
        public float ProduceTotalDiscountAndFavorableCost { get; set; }
        public float ProduceCost { get; set; }
        public float TotalCost { get; set; }
        public string HowBalanceName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Healthline { get; set; }
        public string WebSiteUrl { get; set; }
        public byte HowBalance { get; set; }
        public float RealPrice { get; set; }
        public float ReturnedPrice { get; set; }
        public float MemberCardCashTotal { get; set; }

    }
}
