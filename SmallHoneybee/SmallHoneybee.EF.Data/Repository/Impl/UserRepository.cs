using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmallHoneybee.DataModel.Model;

namespace SmallHoneybee.EF.Data.Repository.Impl
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(IDatabaseContext databaseContext)
            : base(databaseContext)
        {
        }

        public User GetByUserId(int userId)
        {
            return _dataContext.Users.Single(x => x.UserId == userId);
        }
    }
}
