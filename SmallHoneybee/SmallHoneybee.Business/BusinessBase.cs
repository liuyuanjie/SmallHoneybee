using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.EF.Data;

namespace SmallHoneybee.Business
{
    public abstract class BusinessBase
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected BusinessBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
