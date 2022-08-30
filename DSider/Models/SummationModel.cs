using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class SummationModel
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string levelNumber { get; set; }
        public string subProjectID { get; set; }
        public string parentNodeName { get; set; }
        public string parentNodeID { get; set; }
        public int Year { get; set; }
        public double Scope_2_consumption { get; set; }
        public double Scope_1_emission { get; set; }
        public double Scope_3 { get; set; }
        public double OpEx { get; set; }
        public double CapEx { get; set; }
        public string dataType { get; set; }//Achive, BaseLine
    }
}
