using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminService.Model
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IMongoUserDBContext context) : base(context)
        {

        }
    }
}
