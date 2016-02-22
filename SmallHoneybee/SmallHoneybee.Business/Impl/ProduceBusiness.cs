using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;
using SmallHoneybee.EF.Data.Repository;

namespace SmallHoneybee.Business.Impl
{
    public class ProduceBusiness : BusinessBase, IProduceBusiness
    {
        private readonly IProduceRepository _produceRepository;
        public ProduceBusiness(IProduceRepository produceRepository, IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _produceRepository = produceRepository;
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
            _produceRepository.BatchInsertProduces(produces);
            _unitOfWork.Commit();
        }
    }
}
