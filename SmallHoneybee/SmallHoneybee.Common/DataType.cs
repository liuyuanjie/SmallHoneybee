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
            HightManger,
            Admin
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
            Received,
            Completed,
            Archived
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
            ProduceNo,
            PriceOrdered,
            PriceReceived,
            QuantityOrdered,
            QuantityReceived
        }
    }
}
