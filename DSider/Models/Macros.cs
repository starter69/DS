using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace DSider.Models
{
    public class Macros
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string userCreator { get; set; }
        public string createDate { get; set; }
        public List<Components> components { get; set; }
    }
}
