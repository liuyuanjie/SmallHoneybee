using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository
{
    public interface ISOProduceRepository : IRepository<SOProduce>
    {
        SOProduce GetBySOProduceId(int soProduceId);

        void BatchInsertSOProduces(IEnumerable<SOProduce> saleOrders);

        void BatchUpdateSOProduces(IEnumerable<SOProduce> saleOrders);
    }
}
