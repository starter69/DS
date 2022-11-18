using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class DashboardStatistics
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string subProjectID { get; set; }
        public string capex { get; set; }
        public string taxCredit { get; set; }
        public string carbonTax { get; set; }
        public string emissionMitgated { get; set; }
        public string npv { get; set; }
        public string irr { get; set; }
        public string supplyReliability { get; set; }
        public string lcoh { get; set; }
        public string lcoe { get; set; }
        public string variable { get; set; }
        public string period { get; set; }
        public string other { get; set; }
    }
}
