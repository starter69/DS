using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DSider.Models
{
    public class SubProjects
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string projectID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string userCreator { get; set; }
        public string createDate { get; set; }
        public string exportJSON { get; set; }
        public string workFlowStatus { get; set; }
        public string subProject_Type { get; set; }
        public string workspaceZoomLevel { get; set; }
        public string workspaceTranslateX { get; set; }
        public string workspaceTranslateY { get; set; }
    }
}
