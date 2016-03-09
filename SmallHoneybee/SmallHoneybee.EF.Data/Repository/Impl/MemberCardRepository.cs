using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class MemberCardRepository : RepositoryBase<MemberCard>, IMemberCardRepository
    {
        public MemberCardRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public MemberCard GetByMemberCardId(int memberCardId)
        {
            return _dataContext.MemberCards.Single(x => x.MemberCardId == memberCardId);
        }
    }
}
