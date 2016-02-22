using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        PurchaseOrder GetByPurchaseOrderId(int produceId);

        void BatchInsertPurchaseOrders(IEnumerable<PurchaseOrder> purchaseOrders);

        void BatchUpdatePurchaseOrders(IEnumerable<PurchaseOrder> purchaseOrders);
    }
}
