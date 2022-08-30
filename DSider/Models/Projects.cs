using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DSider.Models
{
    public class Projects
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string projectID { get; set; }
        public string name { get; set; }
        public string descrption { get; set; }
        public string userCreator { get; set; }
        public string createDate { get; set; }
    }
}
