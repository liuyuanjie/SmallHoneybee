using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository
{
    public interface IProduceRepository : IRepository<Produce>
    {
        Produce GetByProduceId(int produceId);

        void BatchInsertProduces(IEnumerable<Produce> produces);

        void BatchUpdateProduces(IEnumerable<Produce> produces);
    }
}
