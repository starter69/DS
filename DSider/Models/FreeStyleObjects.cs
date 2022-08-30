using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{

    public class FreeStyleObjects
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string inputCount { get; set; }
        public string OutputCount { get; set; }
        public string uniName { get; set; }
        public string objectType { get; set; }//userDeFined,...
        public string defaultValue { get; set; }
        public string script { get; set; }
        public string imagePath { get; set; }
        public string userCreator { get; set; }
        public string createDate { get; set; }
    }
}
