using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class users
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string userID { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string tokenValidationTime { get; set; }
    }
}
