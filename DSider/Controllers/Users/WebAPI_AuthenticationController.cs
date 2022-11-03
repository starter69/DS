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

namespace DSider.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPI_AuthenticationController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        AppSettings AppSetting;
        public WebAPI_AuthenticationController(IOptions<DatabaseSettings> app)
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
        //Authentication users by user name and password
        //This data sent from LoginController.js
        [HttpPost]
        public users Login([FromBody] users UserInfo)
        {
            users currentUser = new users();
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filterUser = Builders<users>.Filter.Where(c => c.userName.ToLower() == UserInfo.userName.ToLower());
                var collection = mongoDatabase.GetCollection<users>("users").Find(filterUser).ToList();
                if (collection.Count > 0)
                {
                    currentUser = collection[0];
                    var filter = Builders<users>.Filter.Where(p => p.userID == currentUser.userID);
                    var updatestatement = Builders<users>.Update.Set("tokenValidationTime", DateTime.Now.AddDays(5));
                    var result = mongoDatabase.GetCollection<users>("users").UpdateMany(filter, updatestatement);
                } else
                {
                    currentUser.userID = ObjectId.GenerateNewId().ToString();
                    currentUser.userName = UserInfo.userName;
                    currentUser.tokenValidationTime = DateTime.Now.AddDays(5).ToString();
                    mongoDatabase.GetCollection<users>("users").InsertOne(currentUser);
                    AppSetting.saveUserLog(currentUser.userName.ToLower(), "User", "New User", currentUser.userID);
                }
            }
            catch (Exception M)
            {
                string t = M.Message;
            }
            return currentUser;
        }
    }
}
