using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class POItemRepository : RepositoryBase<POItem>, IPOItemRepository
    {
        public POItemRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public POItem GetByPOItemId(int poItemId)
        {
            return _dataContext.POItems.Single(x => x.POItemId == poItemId);
        }

        public void BatchUpdatePOItems(IEnumerable<POItem> poItems)
        {
            throw new NotImplementedException();
        }

        public void BatchInsertPOItems(IEnumerable<POItem> poItems)
        {
            poItems.ToList().ForEach(x =>
            {
                x.CreatedBy = "admin";
                x.CreatedOn = DateTime.Now;
                x.LastModifiedBy = "admin";
                x.LastModifiedOn = DateTime.Now;
                x.RowVersion = DateTime.Now;
                //x.BarCode = !string.IsNullOrEmpty(x.BarCode) ? x.BarCode : x.ProduceNo;
                //x.Enable = true;
            });
            //_dataContext.POItems.AddRange(produces);
        }
    }
}
