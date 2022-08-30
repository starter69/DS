using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class ComponentProperties
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string component { get; set; }
        public string propertyName { get; set; }
        public string propertyValue { get; set; }
        public string propertyType { get; set; }
        public List<string> propertyList { get; set; }
        public string unit { get; set; }
        public string formulaTitle { get; set; }
        public string hasDatasource { get; set; }
        public string description { get; set; }
        public string paramType { get; set; }
    }
}
