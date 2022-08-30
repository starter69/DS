using DSider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPI_ComponentCommentsController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        AppSettings AppSetting;
        //Initialize MongoDB Connections From appsettings.json
        public WebAPI_ComponentCommentsController(IOptions<DatabaseSettings> app)
        {
            appSettings = app;
            AppSetting = new AppSettings(app);
        }
        public IMongoDatabase GetMongoDatabase()
        {
            string ConnectionString = appSettings.Value.ConnectionString;
            string DbName = appSettings.Value.DatabaseName;
            var mongoClient = new MongoClient(ConnectionString);
            return mongoClient.GetDatabase(DbName);
        }
        //Save comments on components whith ajax calling in SimulationController.js
        [Route("saveComments")]
        [HttpPost]
        public void saveComments([FromBody] ComponentComments componentComment)
        {
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //
                mongoDatabase = GetMongoDatabase();
                ObjectId mID = ObjectId.GenerateNewId();
                //
                componentComment.registerDate = DateTime.Now.ToString();
                componentComment.registerUser = userName;
                componentComment._id = mID;
                componentComment.id = mID.ToString();
                //
                mongoDatabase.GetCollection<ComponentComments>("ComponentComments").InsertOne(componentComment);
                //
                AppSetting.saveUserLog(userName.ToLower(), "Comments", "Save Commnents", componentComment.subProjectID);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Load components comments with sub project id.
        [Route("getComponentComments/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public List<ComponentComments> getComponentComments(string subProjectID)
        {
            List<ComponentComments> resultList = new List<ComponentComments>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<ComponentComments>.Filter.Eq("subProjectID", subProjectID);
                resultList = mongoDatabase.GetCollection<ComponentComments>("ComponentComments").Find(filter).ToList();
            }
            catch (Exception)
            {
            }
            return resultList;
        }
    }
}
