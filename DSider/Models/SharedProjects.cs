using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DSider.Models
{
    public class SharedProjects
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string subProjectID { get; set; }
        public string sharedByUserName { get; set; }
        public string sharedToUserName { get; set; }
        public string createDate { get; set; }
        public string subProjectName { get; set; }
        public string permission { get; set; }
        public string sharedProjectStatus { get; set; }
    }
}
