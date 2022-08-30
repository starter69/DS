using DSider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace DSider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPI_DesignController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        AppSettings AppSetting;
        public WebAPI_DesignController(IOptions<DatabaseSettings> app)
        {
            appSettings = app;
            AppSetting = new AppSettings(app);
        }
        //Initialize MongoDB Connections From appsettings.json
        public IMongoDatabase GetMongoDatabase()
        {
            string ConnectionString = appSettings.Value.ConnectionString;
            string DbName = appSettings.Value.DatabaseName;
            var mongoClient = new MongoClient(ConnectionString);
            return mongoClient.GetDatabase(DbName);
        }
        //Save model components thar dragged and dropped in simulation page in work area.
        //When click on save button in SimulationController.js this event will be fire.
        [Route("saveTemplateDesign")]
        [HttpPost]
        public void saveTemplateDesign([FromBody] List<Components> mComponents)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //
                var filter = Builders<Components>.Filter.Eq("subProjectID", mComponents[0].subProjectID);
                var result = mongoDatabase.GetCollection<Components>("Components").DeleteMany(filter);
                //
                foreach (var item in mComponents)
                {
                    ObjectId mID = ObjectId.GenerateNewId();
                    item.creationTime = DateTime.Now.ToString();
                    item.userRegister = userName;
                    item._id = mID;
                    item.id = mID.ToString();
                    mongoDatabase.GetCollection<Components>("Components").InsertOne(item);
                }
                AppSetting.saveUserLog(userName.ToLower(), "Simulation", "save Simulation", mComponents[0].subProjectID);
                //starting date as default property
                //lat & lng as default property
            }
            catch (Exception)
            {
                throw;
            }
        }
        //After saving model components, json template will be save on SubProjects collection in MongoDB.
        //All models will be load with json file.
        [Route("saveExportDataToSubProject")]
        [HttpPost]
        public void saveExportDataToSubProject([FromBody] exportData mData)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<SubProjects>.Filter.Eq("id", mData.subProjectID);
                var updatestatement = Builders<SubProjects>.Update.Set("exportJSON", mData.exportJSON).Set("workspaceZoomLevel", mData.workspaceZoomLevel)
                    .Set("workspaceTranslateX", mData.workspaceTranslateX).Set("workspaceTranslateY", mData.workspaceTranslateY);
                var result = mongoDatabase.GetCollection<SubProjects>("SubProjects").UpdateMany(filter, updatestatement);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Get json template of current model.
        //Drawflow load model with json file.
        [Route("getExportData/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public SubProjects getExportData(string subProjectID)
        {
            string exportData = "";
            List<SubProjects> mList = new List<SubProjects>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<SubProjects>.Filter.Eq("id", subProjectID);
                mList = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filter).ToList();
                if (mList.Count > 0)
                {
                    if (mList[0].exportJSON != null)
                        exportData = mList[0].exportJSON;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return mList[0];
        }
        //Get all info of current sub project.
        [Route("getSubProjectDetails/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public List<SubProjects> getSubProjectDetails(string subProjectID)
        {
            List<SubProjects> resultList = new List<SubProjects>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<SubProjects>.Filter.Eq("id", subProjectID);
                resultList = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filter).ToList();

            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }
        //Each component has own propertiese. 
        //This event get properties of component
        [Route("getComponentsProperties")]
        [HttpGet]
        public List<ComponentProperties> getComponentsProperties()
        {
            List<ComponentProperties> resultList = new List<ComponentProperties>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<ComponentProperties>.Filter.Where(p => p.formulaTitle != "");
                resultList = mongoDatabase.GetCollection<ComponentProperties>("ComponentProperties").Find(filter).SortBy(p => p.propertyName).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }
        //Save model levels in ComponentDetailsModel collection MongoDB.
        //Each level has own zoom level and json data.
        [Route("saveComponentDetailsModel")]
        [HttpPost]
        public bool saveComponentDetailsModel([FromBody] ComponentDetailsModel componentDetail)
        {
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<ComponentDetailsModel>.Filter.Where(p => p.subProjectID == componentDetail.subProjectID && p.nodeID == componentDetail.nodeID &&
                p.nodeName == componentDetail.nodeName);
                List<ComponentDetailsModel> mList = mongoDatabase.GetCollection<ComponentDetailsModel>("ComponentDetailsModel").Find(filter).ToList();
                if (mList.Count == 0)
                {
                    ObjectId mID = ObjectId.GenerateNewId();
                    componentDetail.creationTime = DateTime.Now.ToString();
                    componentDetail.userRegister = userName;
                    componentDetail._id = mID;
                    componentDetail.id = mID.ToString();
                    mongoDatabase.GetCollection<ComponentDetailsModel>("ComponentDetailsModel").InsertOne(componentDetail);
                }
                else
                {
                    var updatestatement = Builders<ComponentDetailsModel>.Update.Set("exportJSON", componentDetail.exportJSON)
                        .Set("workspaceZoomLevel", componentDetail.workspaceZoomLevel).Set("workspaceTranslateX", componentDetail.workspaceTranslateX)
                        .Set("workspaceTranslateY", componentDetail.workspaceTranslateY);
                    var result = mongoDatabase.GetCollection<ComponentDetailsModel>("ComponentDetailsModel").UpdateMany(filter, updatestatement);
                }
            }
            catch (Exception)
            {
            }
            return true;
        }
        //Get model level details.
        [Route("getComponentsDetailsModel/{subProjectID}/{nodeID}/{nodeName}/{levelNumber}")]
        [HttpGet("{subProjectID}/{nodeID}/{nodeName}/{levelNumber}")]
        public ComponentDetailsModel getComponentsDetailsModel(string subProjectID, string nodeID, string nodeName, string levelNumber)
        {
            string exportData = "";
            List<ComponentDetailsModel> mList = new List<ComponentDetailsModel>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<ComponentDetailsModel>.Filter.Where(p => p.subProjectID == subProjectID && p.nodeID == nodeID && p.nodeName == nodeName &&
                p.levelNumber == levelNumber);
                mList = mongoDatabase.GetCollection<ComponentDetailsModel>("ComponentDetailsModel").Find(filter).ToList();
                if (mList.Count > 0)
                {
                    if (mList[0].exportJSON != null)
                        exportData = mList[0].exportJSON;
                }
            }
            catch (Exception)
            {
            }
            return mList.Count > 0 ? mList[0] : null;// exportData == "" ? null : exportData;
        }
        //
        [Route("fakeUpdateComponentProperties")]
        [HttpGet]
        public void fakeUpdateComponentProperties()
        {
            try
            {
                return;
                mongoDatabase = GetMongoDatabase();
                //var filter1 = Builders<ComponentProperties>.Filter.Eq("propertyName", "CapEx");
                var filter1 = Builders<ComponentProperties>.Filter.Where(p => p.propertyName == "CapEx" || p.propertyName == "Scope 1 emission" ||
                p.propertyName == "Scope 2 consumption" || p.propertyName == "Scope 3" || p.propertyName == "OpEx");
                List<ComponentProperties> mList = mongoDatabase.GetCollection<ComponentProperties>("ComponentProperties").Find(filter1).ToList();
                foreach (var item in mList)
                {
                    string mPropertyName = item.propertyName;
                    var filter = Builders<ComponentProperties>.Filter.Eq("id", item.id);
                    var updatestatement = Builders<ComponentProperties>.Update.Set("hasDatasource", "true");
                    var result = mongoDatabase.GetCollection<ComponentProperties>("ComponentProperties").UpdateMany(filter, updatestatement);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        [Route("fakeDuplicateAchiveProperties")]
        [HttpGet]
        public void fakeDuplicateAchiveProperties()
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter1 = Builders<ComponentProperties>.Filter.Where(p => p.propertyName.Contains("Achive"));
                List<ComponentProperties> mList = mongoDatabase.GetCollection<ComponentProperties>("ComponentProperties").Find(filter1).ToList();
                foreach (var item in mList)
                {
                    string mPropertyName = item.propertyName;
                    var filter = Builders<ComponentProperties>.Filter.Eq("id", item.id);
                    var updatestatement = Builders<ComponentProperties>.Update.Set("propertyName", item.propertyName.Replace("Achive", "Achieve"))
                        .Set("formulaTitle", item.formulaTitle.Replace("Achive", "Achieve"));
                    var result = mongoDatabase.GetCollection<ComponentProperties>("ComponentProperties").UpdateMany(filter, updatestatement);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        [Route("fakeDuplicateDataSources")]
        [HttpGet]
        public void fakeDuplicateDataSources()
        {
            return;
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter1 = Builders<DataSources>.Filter.Where(p => p.subProjectID == "623aae7d9738c0a4b356800c");
                List<DataSources> mList = mongoDatabase.GetCollection<DataSources>("DataSources").Find(filter1).ToList();
                //
                foreach (var item in mList)
                {
                    ObjectId mID = ObjectId.GenerateNewId();
                    item._id = mID;
                    item.id = mID.ToString();
                    item.subProjectID = "6243e7856e55535301b8e65e";
                    mongoDatabase.GetCollection<DataSources>("DataSources").InsertOne(item);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public class exportData
    {
        public string subProjectID { get; set; }
        public string exportJSON { get; set; }
        public string workspaceZoomLevel { get; set; }
        public string workspaceTranslateX { get; set; }
        public string workspaceTranslateY { get; set; }
    }
}
