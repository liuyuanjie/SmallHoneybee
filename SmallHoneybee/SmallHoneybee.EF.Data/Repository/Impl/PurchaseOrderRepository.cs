using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class PurchaseOrderRepository : RepositoryBase<PurchaseOrder>, IPurchaseOrderRepository
    {
        public PurchaseOrderRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public PurchaseOrder GetByPurchaseOrderId(int purchaseOrderId)
        {
            return _dataContext.PurchaseOrders.Single(x => x.PurchaseOrderId == purchaseOrderId);
        }

        public void BatchUpdatePurchaseOrders(IEnumerable<PurchaseOrder> purchaseOrders)
        {
            throw new NotImplementedException();
        }

        public void BatchInsertPurchaseOrders(IEnumerable<PurchaseOrder> purchaseOrders)
        {
            purchaseOrders.ToList().ForEach(x =>
            {
                x.CreatedBy = "admin";
                x.CreatedOn = DateTime.Now;
                x.LastModifiedBy = "admin";
                x.LastModifiedOn = DateTime.Now;
                x.RowVersion = DateTime.Now;

                switch ((DataType.POStatusCategory)x.POStatusCategory)
                {
                    case DataType.POStatusCategory.Ordered:
                        if (!x.DateOriginated.HasValue)
                        {
                            x.DateOriginated = DateTime.Now;
                        }
                        break;

                    case DataType.POStatusCategory.Completed:
                        if (!x.DateCompleted.HasValue)
                        {
                            x.DateCompleted = DateTime.Now;
                        }
                        break;
                }
            });
            _dataContext.PurchaseOrders.AddRange(purchaseOrders);
        }
    }
}
