using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class ProduceRepository : RepositoryBase<Produce>, IProduceRepository
    {
        public ProduceRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public Produce GetByProduceId(int produceId)
        {
            return _dataContext.Produces.Single(x => x.ProduceId == produceId);
        }

        public void BatchUpdateProduces(IEnumerable<Produce> produces)
        {
            throw new NotImplementedException();
        }

        public void BatchInsertProduces(IEnumerable<Produce> produces)
        {
            produces.ToList().ForEach(x =>
            {
                x.CreatedBy = "admin";
                x.CreatedOn = DateTime.Now;
                x.LastModifiedBy = "admin";
                x.LastModifiedOn = DateTime.Now;
                x.RowVersion = DateTime.Now;
                x.BarCode = !string.IsNullOrEmpty(x.BarCode) ? x.BarCode : x.ProduceNo;
                x.Enable = true;
            });
            _dataContext.Produces.AddRange(produces);
        }
    }
}
