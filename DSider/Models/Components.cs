using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class Components
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string subProjectID { get; set; }
        public string nodeID { get; set; }
        public string nodeName { get; set; }
        public string type { get; set; }//turbine, storage
        public string positionTop { get; set; }
        public string positionLeft { get; set; }
        public List<nodeConnections> inputConnections { get; set; }
        public List<nodeConnections> outputConnections { get; set; }
        public List<ComponentProperty> Propertiese { get; set; }
        public string nodeHtmlContent { get; set; }
        public string creationTime { get; set; }
        public string IPAddress { get; set; }
        public string userRegister { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string priority { get; set; }
        public string inputConnectionsCount { get; set; }
        public string outputConnectionsCount { get; set; }

    }
}