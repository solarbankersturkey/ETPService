using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminService.Model
{
    public interface IMongoUserDBContext
    {
        IMongoCollection<User> GetCollection<User>(string name);
    }
}
