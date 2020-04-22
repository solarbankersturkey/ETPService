using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Model
{
    public interface IMongoUserDBContext
    {
        IMongoCollection<User> GetCollection<User>(string name);
    }
}
