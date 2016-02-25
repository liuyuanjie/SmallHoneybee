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
            Worker,
            Admin
        }

        public enum DayBookType
        {
            None,
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
