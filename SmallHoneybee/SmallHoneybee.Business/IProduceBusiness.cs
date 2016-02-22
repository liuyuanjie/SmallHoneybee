using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.Business
{
    public interface IProduceBusiness
    {
        void BatchInsertProduces(IEnumerable<Produce> produces);
    }
}
