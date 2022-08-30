using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using DSider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace DSider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPI_DataSourceController : ControllerBase
    {
        private readonly AppSettings AppSetting;
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        //Initialize MongoDB Connections From appsettings.json
        public WebAPI_DataSourceController(IOptions<DatabaseSettings> app)
        {
            appSettings = app;
            AppSetting = new AppSettings(appSettings);
        }
        public IMongoDatabase GetMongoDatabase()
        {
            string ConnectionString = appSettings.Value.ConnectionString;
            string DbName = appSettings.Value.DatabaseName;
            var mongoClient = new MongoClient(ConnectionString);
            return mongoClient.GetDatabase(DbName);
        }
        //Get all datasources in current sub project with sub project id
        [Route("selectDataSources/{subProjecID}")]
        [HttpGet("{subProjecID}")]
        public List<DataSources> selectDataSources(string subProjecID)
        {
            mongoDatabase = GetMongoDatabase();
            List<DataSources> resultList = new List<DataSources>();
            try
            {
                var filter = Builders<DataSources>.Filter.Eq("subProjectID", subProjecID);
                resultList = mongoDatabase.GetCollection<DataSources>("DataSources").Find(filter).ToList();
            }
            catch (Exception)
            {
            }
            return resultList;
        }
        //Delete selected datasource with id
        [Route("deleteDataSource/{id}")]
        [HttpGet("{id}")]
        public void deleteDataSource(string id)
        {
            try
            {
                string userName = Request.Cookies["userName"];
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<DataSources>.Filter.Eq("id", id);
                var result = mongoDatabase.GetCollection<DataSources>("DataSources").DeleteMany(filter);
                AppSetting.saveUserLog(userName.ToLower(), "Simulation", "deleteDataSource", "");

            }
            catch (Exception)
            {
            }
        }
    }
}
