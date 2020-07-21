using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Contexts;
using CustomerService.Model;

namespace CustomerService.Repositories {
    public class UserRepository : BaseRepository<User>, IUserRepository {
        public UserRepository (IMongoUserDBContext context) : base (context) {

        }
    }
}
