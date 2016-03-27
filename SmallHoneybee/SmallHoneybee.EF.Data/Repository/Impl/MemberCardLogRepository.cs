using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class MemberCardLogRepository : RepositoryBase<MemberCardLog>, IMemberCardLogRepository
    {
        public MemberCardLogRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }
    }
}
