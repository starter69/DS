using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class ComponentComments
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string subProjectID { get; set; }
        public string nodeID { get; set; }
        public string componentName { get; set; }
        public string commentText { get; set; }
        public string registerDate { get; set; }
        public string registerUser { get; set; }
    }
}