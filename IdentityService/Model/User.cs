using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("Username")]
        public string Username { get; set; }
        [BsonElement("Password")]
        public string Password { get; set; }
        [BsonElement("OnlineStatus")]
        public bool OnlineStatus { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Surname")]
        public string Surname { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; }
        [BsonElement("Profile")]
        public Profile Profile { get; set; }
        [BsonElement("Status")]
        public string Status { get; set; }
        [BsonElement("Wallet_ID")]
        public string Wallet_ID { get; set; }
        [BsonElement("Type")]
        public string Type { get; set; }
        [BsonElement("Detail")]
        public Detail Detail { get; set; }
        [BsonElement("InstalledPower")]
        public InstalledPower InstalledPower { get; set; }
    }
}
