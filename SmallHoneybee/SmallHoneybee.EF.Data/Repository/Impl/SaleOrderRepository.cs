using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class SaleOrderRepository : RepositoryBase<SaleOrder>, ISaleOrderRepository
    {
        public SaleOrderRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public SaleOrder GetBySaleOrderId(int saleOrderId)
        {
            return _dataContext.SaleOrders.Single(x => x.SaleOrderId == saleOrderId);
        }

        public void BatchUpdateSaleOrders(IEnumerable<SaleOrder> produces)
        {
            throw new NotImplementedException();
        }

        public void BatchInsertSaleOrders(IEnumerable<SaleOrder> produces)
        {
            
        }
    }
}
