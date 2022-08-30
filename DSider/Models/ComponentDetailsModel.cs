using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class ComponentDetailsModel
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string subProjectID { get; set; }
        public string nodeID { get; set; }
        public string nodeName { get; set; }
        public string creationTime { get; set; }
        public string IPAddress { get; set; }
        public string userRegister { get; set; }
        public string exportJSON { get; set; }
        public string levelNumber { get; set; }
        public string workspaceZoomLevel { get; set; }
        public string workspaceTranslateX { get; set; }
        public string workspaceTranslateY { get; set; }

    }
}