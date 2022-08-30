using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DSider.Models
{
    public class UserLog
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string userName { get; set; }
        public string activity { get; set; }
        public string dateTime { get; set; }
        public string pageName { get; set; }
        public string subProjectID { get; set; }
    }
}
