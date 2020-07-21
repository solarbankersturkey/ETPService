using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerService.Model;
using MongoDB.Driver;

namespace CustomerService.Contexts {
    public interface IMongoUserDBContext {
        IMongoCollection<User> GetCollection<User> (string name);
    }
}
