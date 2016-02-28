using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallHoneybee.Common
{
    public class RolePermission
    {
        public RolePermission(DataType.UserType userType)
        {
            switch (userType)
            {
                case DataType.UserType.GeneralManger:
                    SaleOrderView = true;
                    SaleOrderEdit = true;
                    SaleOrderDelete = true;
                    SaleOrderFavorableLimitCost = true;

                    ProduceView = true;
                    ProduceEdit = true;
                    ProduceHeightEdit = false;
                    ProduceFactoryPriceEdit = false;
                    ProduceDelete = false;
                    PurchaseOrderView = false;
                    PurchaseOrderEdit = false;
                    PurchaseOrderDelete = false;
                    UserView = true;
                    UserEdit = true;
                    UserDelete = false;
                    DayBookView = false;
                    DayBookEdit = false;
                    DayBookDelete = false;
                    SystemSettingView = false;
                    SystemSettingEdit = false;
                    SystemSettingDelete = false;
                    break;

                case DataType.UserType.HightManger:
                    SaleOrderView = true;
                    SaleOrderEdit = true;
                    SaleOrderDelete = true;
                    SaleOrderFavorableCost = true;

                    ProduceView = true;
                    ProduceEdit = true;
                    ProduceHeightEdit = true;
                    ProduceFactoryPriceEdit = false;
                    ProduceDelete = true;
                    PurchaseOrderView = false;
                    PurchaseOrderEdit = false;
                    PurchaseOrderDelete = false;
                    ProduceFactoryPriceEdit = false;
                    UserView = true;
                    UserEdit = true;
                    UserDelete = true;
                    DayBookView = true;
                    DayBookEdit = true;
                    DayBookDelete = false;
                    SystemSettingView = false;
                    SystemSettingEdit = false;
                    SystemSettingDelete = false;
                    break;

                case DataType.UserType.Admin:
                    SaleOrderView = true;
                    SaleOrderEdit = true;
                    SaleOrderDelete = true;
                    SaleOrderDelete = true;
                    SaleOrderFavorableCost = true;

                    ProduceView = true;
                    ProduceEdit = true;
                    ProduceHeightEdit = true;
                    ProduceFactoryPriceEdit = true;
                    ProduceDelete = true;

                    PurchaseOrderView = true;
                    PurchaseOrderEdit = true;
                    PurchaseOrderDelete = true;

                    UserView = true;
                    UserEdit = true;
                    UserDelete = true;

                    DayBookView = true;
                    DayBookEdit = true;
                    DayBookDelete = true;

                    SystemSettingView = true;
                    SystemSettingEdit = true;
                    SystemSettingDelete = true;
                    break;
            }
        }

        public bool SaleOrderFavorableLimitCost { get; set; }

        public bool SaleOrderFavorableCost { get; set; }
        public bool SaleOrderView { get; set; }
        public bool SaleOrderEdit { get; set; }
        public bool SaleOrderDelete { get; set; }

        public bool ProduceView { get; set; }
        public bool ProduceEdit { get; set; }
        public bool ProduceHeightEdit { get; set; }
        public bool ProduceFactoryPriceEdit { get; set; }
        public bool ProduceDelete { get; set; }

        public bool PurchaseOrderView { get; set; }
        public bool PurchaseOrderEdit { get; set; }
        public bool PurchaseOrderDelete { get; set; }

        public bool UserView { get; set; }
        public bool UserEdit { get; set; }
        public bool UserDelete { get; set; }

        public bool DayBookView { get; set; }
        public bool DayBookEdit { get; set; }
        public bool DayBookDelete { get; set; }

        public bool SystemSettingView { get; set; }
        public bool SystemSettingEdit { get; set; }
        public bool SystemSettingDelete { get; set; }
    }
}
