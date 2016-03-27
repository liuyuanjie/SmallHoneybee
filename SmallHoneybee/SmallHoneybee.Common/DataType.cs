using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallHoneybee.Common
{
    public class DataType
    {
        public enum UserType
        {
            Coustom,
            GeneralManger,
            HightManger = 30,
            FactoryPriceManger = 60,
            Admin = 88
        }

        public enum DayBookType
        {
            PayOut,
            InCome
        }

        public enum POItemStatusCategory
        {
            None,
            Dispatched
        }

        public enum ProduceUnit
        {
            Box,
            Bottle,
            Piece,
            Branch,
            Case,
            Bag
        }

        public enum MemberType
        {
            Junior,
            Higher = 10,
            Middle = 20,
            Top = 30
        }

        public enum ProduceField
        {
            Category,
            ProduceNo,
            Name,
            HowUsed,
            FactoryPrice,
            RetailPrice
        }

        public enum SaleOrderBalancedMode
        {
            Cash,
            MemberCard,
            UnitUnionPay,
            WeiChat
        }

        public enum POStatusCategory
        {
            Ordered,
            Completed
        }

        public enum SaleOrderStatus
        {
            NotBalanced,
            Balanced,
        }

        public enum ImporterType
        {
            Produce,
            PurchaseOrder
        }

        public enum PurchaseOrderField
        {
            POContractNo,
            PurchaseOrderNo,
            ProduceName,
            ProduceNo,
            PriceOrdered,
            PriceReceived,
            QuantityOrdered,
            QuantityReceived
        }

        public enum SystemSettingCode : short
        {
            SystemVersion = 1000,
            MemberPointsRate = 1001,
            SOProduceGeneralMangerMaxDiscountPrice = 1002,
            DefaultMemberCardPW=1003,

            ReportMerchantsName = 1101,
            ReportPhone = 1102,
            ReportAddress = 1103,
            ReportPrintName = 1104,
            ReportHealthline = 1105,
            ReportWebSiteUrl = 1106,
        }

        public enum MemberCardStatus
        {
            NonActive,
            Active
        }

        public enum MemberCardLogType 
        {
            Other,
            Saved,
            Consumption
        }

        public enum UserLogType
        {
            Other,
            Saved,
            Consumption
        }

        public enum ProducelogType
        {
            Other,
            SaleOrder,
            PurchaseOrder
        }
    }
}
