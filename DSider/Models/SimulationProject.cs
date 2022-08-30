using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class SimulationProject
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string projectID { get; set; }
        public string projectName { get; set; }
        public string subProjectID { get; set; }
        public string subProjectName { get; set; }
        public List<Components> componentsInfo { get; set; }
    }
}
