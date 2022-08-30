using Microsoft.Extensions.Options;
using MongoDB.Driver;
using DSider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace DSider.Controllers
{
    public class AppSettings
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        public AppSettings(IOptions<DatabaseSettings> app)
        {
            appSettings = app;

        }
        //Initialize MongoDB Connections From appsettings.json
        public IMongoDatabase GetMongoDatabase()
        {
            string ConnectionString = appSettings.Value.ConnectionString;
            string DbName = appSettings.Value.DatabaseName;
            var mongoClient = new MongoClient(ConnectionString);
            return mongoClient.GetDatabase(DbName);
        }
        //Save Log in MongoDB
        public void saveUserLog(string userName, string pageName, string activity, string subProjectID)
        {
            try
            {
                UserLog mUserLog = new UserLog();
                mongoDatabase = GetMongoDatabase();
                ObjectId mID = ObjectId.GenerateNewId();
                mUserLog.id = mID.ToString();
                mUserLog._id = mID;
                mUserLog.activity = activity;
                mUserLog.dateTime = DateTime.Now.ToString();
                mUserLog.userName = userName;
                mUserLog.subProjectID = subProjectID;
                mUserLog.pageName = pageName;
                mongoDatabase.GetCollection<UserLog>("UserLog").InsertOne(mUserLog);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //This event check users validation with token validation
        //If token exprired, user will be redirect to login page
        public users checkUserValidation(string userName)
        {
            users mUser = new users();
            try
            {
                mongoDatabase = GetMongoDatabase();
                var collection = mongoDatabase.GetCollection<users>("users").Aggregate().Match(c => c.userName.ToLower() == userName.ToLower()).ToList();
                if (collection.Count > 0)
                {
                    if (DateTime.Parse(collection[0].tokenValidationTime) >= DateTime.Now)
                        mUser = collection[0];
                }
            }
            catch (Exception)
            {
                return mUser;
            }
            return mUser;
        }
    }
}
