using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IotService.Model
{
    public interface IMongoCumulativeDBContext
    {
        IMongoCollection<Cumulative> GetCollection<Cumulative>(string name);
    }
}
