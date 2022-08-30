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
    public class WebAPI_MacrosController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        AppSettings AppSetting;
        //Initialize MongoDB Connections From appsettings.json
        public WebAPI_MacrosController(IOptions<DatabaseSettings> app)
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
        //On save macro button in SimulationController.js 
        //Macro data will be save in Macros collection in MongoDB
        [Route("saveMacro")]
        [HttpPost]
        public void saveMacro([FromBody] Macros macroInfo)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                //
                ObjectId mID = ObjectId.GenerateNewId();
                macroInfo.createDate = DateTime.Now.ToString();
                macroInfo.userCreator = userName.ToLower();
                macroInfo.id = mID.ToString();
                macroInfo._id = mID;
                mongoDatabase.GetCollection<Macros>("Macros").InsertOne(macroInfo);
                AppSetting.saveUserLog(userName.ToLower(), "Simulation", "save Macro", "");
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Get Macros that logged in user saved
        [Route("getUserMacros")]
        [HttpGet]
        public List<Macros> getUserMacros()
        {
            List<Macros> resultList = new List<Macros>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //
                var filter = Builders<Macros>.Filter.Eq("userCreator", userName.ToLower());
                resultList = mongoDatabase.GetCollection<Macros>("Macros").Find(filter).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }
        //Delete macro with macro id
        [Route("deleteMacroObject/{id}")]
        [HttpGet("{id}")]
        public void deleteMacroObject(string id)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                var filter = Builders<Macros>.Filter.Eq("id", id);
                var result = mongoDatabase.GetCollection<Macros>("Macros").DeleteMany(filter);
                AppSetting.saveUserLog(userName.ToLower(), "Simulation", "Delete Macro", "");

            }
            catch (Exception)
            {
            }
        }
    }
}
