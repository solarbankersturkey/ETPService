using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CustomerService.Dto {
    public class RegisterComplete {
        [BsonRepresentation (BsonType.ObjectId)]
        public string Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string BirthDate { get; set; }
        public string IdentityNumber { get; set; }
        public string InvoiceNumber { get; set; }

    }
}
