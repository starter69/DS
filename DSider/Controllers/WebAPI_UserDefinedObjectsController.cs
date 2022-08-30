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
    public class WebAPI_UserDefinedObjectsController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        AppSettings AppSetting;
        //Initialize MongoDB Connections From appsettings.json
        public WebAPI_UserDefinedObjectsController(IOptions<DatabaseSettings> app)
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
        //When save new user defined object this event save information in FreeStyleObjects collection
        [Route("saveUserDefinedObject")]
        [HttpPost]
        public string saveUserDefinedObject([FromBody] FreeStyleObjects mObject)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                ObjectId mID = ObjectId.GenerateNewId();
                mObject.createDate = DateTime.Now.ToString();
                mObject.userCreator = userName.ToLower();
                mObject._id = mID;
                mObject.id = mID.ToString();
                mongoDatabase.GetCollection<FreeStyleObjects>("FreeStyleObjects").InsertOne(mObject);
                AppSetting.saveUserLog(userName.ToLower(), "Simulation", "Save User Defined Object", mObject.id);

            }
            catch (Exception M)
            {
                string t = M.Message;
                throw;
            }
            return mObject.id;
        }
        //Get user defined object that created by user
        [Route("getUserDefinedObject")]
        [HttpGet]
        public List<FreeStyleObjects> getUserDefinedObject()
        {
            List<FreeStyleObjects> resultList = new List<FreeStyleObjects>();
            try
            {
                string userName = Request.Cookies["userName"];
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<FreeStyleObjects>.Filter.Eq("userCreator", userName.ToLower());
                resultList = mongoDatabase.GetCollection<FreeStyleObjects>("FreeStyleObjects").Find(filter).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }
        //Delete user defined object by id
        [Route("deleteUserDefinedObject/{id}")]
        [HttpGet("{id}")]
        public void deleteUserDefinedObject(string id)
        {
            try
            {
                string userName = Request.Cookies["userName"];
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<FreeStyleObjects>.Filter.Eq("id", id);
                var result = mongoDatabase.GetCollection<FreeStyleObjects>("FreeStyleObjects").DeleteMany(filter);
                AppSetting.saveUserLog(userName.ToLower(), "Simulation", "Delete User Defined Object", id);
            }
            catch (Exception)
            {
            }
        }
    }
}
