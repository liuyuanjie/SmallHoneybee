using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class SOProduceRepository : RepositoryBase<SOProduce>, ISOProduceRepository
    {
        public SOProduceRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public SOProduce GetBySOProduceId(int soProduceId)
        {
            return _dataContext.SOProduces.Single(x => x.SOProduceId == soProduceId);
        }

        public void BatchUpdateSOProduces(IEnumerable<SOProduce> produces)
        {
            throw new NotImplementedException();
        }

        public void BatchInsertSOProduces(IEnumerable<SOProduce> produces)
        {
            
        }
    }
}
