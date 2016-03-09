using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository
{
    public interface IMemberCardRepository : IRepository<MemberCard>
    {
        MemberCard GetByMemberCardId(int memberCardId);
    }
}
