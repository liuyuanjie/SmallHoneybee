using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository
{
    public interface IPOItemRepository : IRepository<POItem>
    {
        POItem GetByPOItemId(int poItemId);

        void BatchInsertPOItems(IEnumerable<POItem> poItems);

        void BatchUpdatePOItems(IEnumerable<POItem> poItems);
    }
}
