using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace IotService.Model
{
    public class Cumulative
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string CustomerId { get; set; }
        public string GatewayId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public dynamic DynamicData { get; set; }
    }
}
