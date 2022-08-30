using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DSider.Models
{
    public class DataSources
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string filePath { get; set; }
        public string description { get; set; }
        public string userCreator { get; set; }
        public string createDate { get; set; }
        public string subProjectID { get; set; }
        public string[] columnsList { get; set; }
    }
}
