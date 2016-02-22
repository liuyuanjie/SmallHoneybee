using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository
{
    public interface ISaleOrderRepository : IRepository<SaleOrder>
    {
        SaleOrder GetBySaleOrderId(int saleOrderId);

        void BatchInsertSaleOrders(IEnumerable<SaleOrder> saleOrders);

        void BatchUpdateSaleOrders(IEnumerable<SaleOrder> saleOrders);
    }
}
