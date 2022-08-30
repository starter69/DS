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
    public class WebAPI_ProjectsController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        AppSettings AppSetting;
        //Initialize MongoDB Connections From appsettings.json
        public WebAPI_ProjectsController(IOptions<DatabaseSettings> app)
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
        //Save project info when click on save project in ProjectsController.js
        //Project data will be save in Projects collection.
        [Route("saveProject/{projectName}/{projectDescription}/{id}")]
        [HttpGet("{projectName}/{projectDescription}/{id}")]
        public string saveProject(string projectName, string projectDescription, string id)
        {
            Projects projectModel = new Projects();
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                projectModel.createDate = DateTime.Now.ToString();
                projectModel.userCreator = userName;
                if (id == "0")
                {
                    projectModel.name = projectName;
                    projectModel.descrption = projectDescription;
                    projectModel.projectID = ObjectId.GenerateNewId().ToString();
                    mongoDatabase.GetCollection<Projects>("Projects").InsertOne(projectModel);
                    AppSetting.saveUserLog(userName.ToLower(), "Project", "Save Project", projectModel.projectID);
                }
                else
                {
                    projectModel.projectID = id;
                    var filter = Builders<Projects>.Filter.Eq("projectID", id);
                    var updatestatement = Builders<Projects>.Update.Set("name", projectName)
                    .Set("descrption", projectDescription).Set("createDate", DateTime.Now.ToString()).Set("userCreator", userName);
                    var result = mongoDatabase.GetCollection<Projects>("Projects").UpdateMany(filter, updatestatement);
                    AppSetting.saveUserLog(userName.ToLower(), "Project", "Edit Project", id);
                }

                return projectModel.projectID;
            }
            catch (Exception)
            {
            }
            return projectModel.projectID;
        }
        //GEt projects info from Projects Collection
        [Route("selectProject")]
        [HttpGet]
        public List<Projects> selectProject()
        {
            mongoDatabase = GetMongoDatabase();
            List<Projects> resultList = new List<Projects>();
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();

                var collection = mongoDatabase.GetCollection<Projects>("Projects").AsQueryable().ToList();
                resultList = collection.Where(p => p.userCreator.ToLower() == userName.ToLower()).ToList();
                //
                var filter = Builders<SharedProjects>.Filter.Eq("sharedToUserName", userName);
                List<SharedProjects> mSharedProjects = mongoDatabase.GetCollection<SharedProjects>("SharedProjects").Find(filter)
                    .ToList();
                if (mSharedProjects.Count > 0)
                {
                    resultList.Add(new Projects()
                    {
                        createDate = mSharedProjects[0].createDate,
                        descrption = "Shared Projects",
                        name = "Shared Projects",
                        projectID = "-1",
                        userCreator = "",
                    });
                }
            }
            catch (Exception)
            {
            }
            return resultList;
        }
        //Delete project by id
        [Route("deleteProject/{projectID}")]
        [HttpGet("{projectID}")]
        public void deleteProject(string projectID)
        {
            try
            {
                string userName = Request.Cookies["userName"];
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<Projects>.Filter.Eq("projectID", projectID);
                var result = mongoDatabase.GetCollection<Projects>("Projects").DeleteMany(filter);
                AppSetting.saveUserLog(userName.ToLower(), "Project", "Delete Project", projectID);

            }
            catch (Exception)
            {
            }
        }
        //Save sub prject info (model info) in SubProject collection
        [Route("saveSubProject/{subProjectName}/{Description}/{projectID}/{id}/{subProject_Type}")]
        [HttpGet("{subProjectName}/{Description}/{projectID}/{id}/{subProject_Type}")]
        public string saveSubProject(string subProjectName, string Description, string projectID, string id, string subProject_Type)
        {
            SubProjects subProjectModel = new SubProjects();
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                mongoDatabase = GetMongoDatabase();

                if (id == "0")
                {
                    subProjectModel.id = ObjectId.GenerateNewId().ToString();
                    subProjectModel.projectID = projectID;
                    subProjectModel.userCreator = userName;
                    subProjectModel.name = subProjectName;
                    subProjectModel.description = Description;
                    subProjectModel.createDate = DateTime.Now.ToString();
                    subProjectModel.subProject_Type = subProject_Type;
                    mongoDatabase.GetCollection<SubProjects>("SubProjects").InsertOne(subProjectModel);
                    AppSetting.saveUserLog(userName.ToLower(), "Project", "Save Sub Project", subProjectModel.id);

                }
                else
                {
                    subProjectModel.id = id;
                    var filter = Builders<SubProjects>.Filter.Eq("id", id);
                    var updatestatement = Builders<SubProjects>.Update.Set("name", subProjectName)
                    .Set("description", Description).Set("createDate", DateTime.Now.ToString()).Set("userCreator", userName).Set("subProject_Type", subProject_Type);
                    var result = mongoDatabase.GetCollection<SubProjects>("SubProjects").UpdateMany(filter, updatestatement);
                    AppSetting.saveUserLog(userName.ToLower(), "Project", "Edit Sub Project", subProjectModel.id);
                }

                return subProjectModel.id;
            }
            catch (Exception)
            {
            }
            return subProjectModel.id;
        }
        //Get sub project info from project id
        [Route("selectSubProject/{projectID}")]
        [HttpGet("{projectID}")]
        public List<SubProjects> selectSubProject(string projectID)
        {
            mongoDatabase = GetMongoDatabase();
            List<SubProjects> resultList = new List<SubProjects>();
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //
                if (projectID != "-1")
                {
                    var collection = mongoDatabase.GetCollection<SubProjects>("SubProjects").AsQueryable().ToList();
                    resultList = collection.Where(p => p.projectID == projectID).ToList();
                }
                else
                {
                    var filter = Builders<SharedProjects>.Filter.Eq("sharedToUserName", userName);
                    List<SharedProjects> mSharedProjects = mongoDatabase.GetCollection<SharedProjects>("SharedProjects").Find(filter).ToList();
                    if (mSharedProjects.Count > 0)
                    {
                        foreach (var item in mSharedProjects)
                        {
                            resultList.Add(new SubProjects()
                            {
                                createDate = item.createDate,
                                name = item.subProjectName,
                                description = "Shared Project",
                                id = item.subProjectID,
                                userCreator = item.sharedByUserName,
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return resultList;
        }
        //Get all sub projects
        [Route("getAllSubProjects")]
        [HttpGet]
        public List<SubProjects> getAllSubProjects()
        {
            mongoDatabase = GetMongoDatabase();
            List<SubProjects> resultList = new List<SubProjects>();
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //
                var filter = Builders<Projects>.Filter.Eq("userCreator", userName);
                var collection = mongoDatabase.GetCollection<Projects>("Projects").Find(filter).ToList();
                foreach (var itemPro in collection)
                {
                    var filterSub = Builders<SubProjects>.Filter.Eq("projectID", itemPro.projectID);
                    var subProjects = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filterSub).ToList();
                    foreach (var item in subProjects)
                    {
                        resultList.Add(new SubProjects()
                        {
                            id = item.id,
                            name = item.name
                        });
                    }

                }

            }
            catch (Exception)
            {
            }
            return resultList;
        }
        //Get projects and sub projects together
        [Route("getProjectAndSubProjectHierarchy")]
        [HttpGet]
        public List<projectAndSubProjectHierarchy> getProjectAndSubProjectHierarchy()
        {
            List<projectAndSubProjectHierarchy> resultList = new List<projectAndSubProjectHierarchy>();
            try
            {
                mongoDatabase = GetMongoDatabase();

                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();

                var collection = mongoDatabase.GetCollection<Projects>("Projects").AsQueryable().ToList();
                List<Projects> projectsList = collection.Where(p => p.userCreator.ToLower() == userName.ToLower()).ToList();
                foreach (var item in projectsList)
                {
                    var collectionSubProjects = mongoDatabase.GetCollection<SubProjects>("SubProjects").AsQueryable().ToList();
                    List<SubProjects> subProjectsList = collectionSubProjects.Where(p => p.projectID == item.projectID).ToList();
                    resultList.Add(new projectAndSubProjectHierarchy()
                    {
                        projectID = item.projectID,
                        projectName = item.name,
                        subProjectsList = subProjectsList.ToList()
                    });
                }

            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }
        //Delete sub project by id
        [Route("deleteSubProject/{id}")]
        [HttpGet("{id}")]
        public void deleteSubProject(string id)
        {
            try
            {
                string userName = Request.Cookies["userName"];
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<SubProjects>.Filter.Eq("id", id);
                var result = mongoDatabase.GetCollection<SubProjects>("SubProjects").DeleteMany(filter);
                AppSetting.saveUserLog(userName.ToLower(), "Project", "Delete Sub Project", id);
            }
            catch (Exception)
            {
            }
        }
        //Share sub project with permission
        //If duplicate = true create new sub project
        [Route("shareSubProject/{subProjectID}/{userNameToShare}/{permission}/{duplicate}")]
        [HttpGet("{subProjectID}/{userNameToShare}/{permission}/{duplicate}")]
        public string shareSubProject(string subProjectID, string userNameToShare, string permission, bool duplicate)
        {
            string messageResul = "";
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //
                mongoDatabase = GetMongoDatabase();
                List<users> collectionUsers = mongoDatabase.GetCollection<users>("users").AsQueryable().ToList();
                collectionUsers = collectionUsers.Where(p => p.userName.ToLower() == userNameToShare.ToLower()).ToList();
                if (collectionUsers.Count > 0)
                {
                    if (duplicate)
                        subProjectID = duplicateSubProject(subProjectID);
                    var filter = Builders<SubProjects>.Filter.Eq("id", subProjectID);
                    List<SubProjects> subProjectList = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filter).ToList();
                    //
                    SharedProjects sharedPorjectModel = new SharedProjects();
                    sharedPorjectModel.id = ObjectId.GenerateNewId().ToString();
                    sharedPorjectModel.subProjectID = subProjectID;
                    sharedPorjectModel.sharedByUserName = userName;
                    sharedPorjectModel.sharedToUserName = userNameToShare.ToLower();
                    sharedPorjectModel.createDate = DateTime.Now.ToString();
                    sharedPorjectModel.subProjectName = subProjectList[0].name;
                    sharedPorjectModel.permission = permission;
                    mongoDatabase.GetCollection<SharedProjects>("SharedProjects").InsertOne(sharedPorjectModel);
                    messageResul = "Shared succeded";
                    AppSetting.saveUserLog(userName.ToLower(), "Project", "Share", subProjectID);

                }
                else
                    messageResul = "UserName is not valid. Try another one.";

            }
            catch (Exception)
            {
                messageResul = "Something wrong";
                throw;
            }
            return messageResul;
        }
        //Get all shared history with sub project id
        //For example you can share sb project with multi users
        [Route("getSharedHistoryBySubProjectID/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public List<SharedProjects> getSharedHistoryBySubProjectID(string subProjectID)
        {
            mongoDatabase = GetMongoDatabase();
            List<SharedProjects> resultList = new List<SharedProjects>();
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                var filter = Builders<SharedProjects>.Filter.Eq("subProjectID", subProjectID);
                resultList = mongoDatabase.GetCollection<SharedProjects>("SharedProjects").Find(filter).ToList();
                resultList = resultList.Where(p => p.sharedByUserName.ToLower() == userName).ToList();
            }
            catch (Exception)
            {
            }
            return resultList;
        }
        //Delete shared sub project
        [Route("deleteSharedProject/{id}")]
        [HttpGet("{id}")]
        public void deleteSharedProject(string id)
        {
            mongoDatabase = GetMongoDatabase();
            try
            {
                string userName = Request.Cookies["userName"];
                var filter = Builders<SharedProjects>.Filter.Eq("id", id);
                var result = mongoDatabase.GetCollection<SharedProjects>("SharedProjects").DeleteMany(filter);
                AppSetting.saveUserLog(userName.ToLower(), "Project", "Delete Share", id);

            }
            catch (Exception)
            {
            }
        }
        //Duplicate current sub project with sub project id
        [Route("duplicateSubProject/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public string duplicateSubProject(string subProjectID)
        {
            string newSubProjectID = "";
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<SubProjects>.Filter.Eq("id", subProjectID);
                List<SubProjects> subProjectsList = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filter).ToList();
                foreach (var item in subProjectsList)
                {
                    SubProjects subProjectModel = new SubProjects();
                    subProjectModel.id = ObjectId.GenerateNewId().ToString();
                    subProjectModel.projectID = item.projectID;
                    subProjectModel.userCreator = userName;
                    subProjectModel.name = item.name + "-Duplicated";
                    subProjectModel.description = item.description;
                    subProjectModel.createDate = DateTime.Now.ToString();
                    subProjectModel.exportJSON = item.exportJSON;
                    subProjectModel.workspaceZoomLevel = item.workspaceZoomLevel;
                    subProjectModel.workspaceTranslateX = item.workspaceTranslateX;
                    subProjectModel.workspaceTranslateY = item.workspaceTranslateY;
                    subProjectModel.workFlowStatus = item.workFlowStatus;
                    subProjectModel.subProject_Type = item.subProject_Type;
                    newSubProjectID = subProjectModel.id;
                    mongoDatabase.GetCollection<SubProjects>("SubProjects").InsertOne(subProjectModel);
                    //
                }
                //ComponentDetailsModel
                var filterComponentDetailsModel = Builders<ComponentDetailsModel>.Filter.Eq("subProjectID", subProjectID);
                List<ComponentDetailsModel> detailModel = mongoDatabase.GetCollection<ComponentDetailsModel>("ComponentDetailsModel").Find(filterComponentDetailsModel).ToList();
                foreach (ComponentDetailsModel componentDetail in detailModel)
                {
                    ObjectId mID = ObjectId.GenerateNewId();
                    componentDetail.creationTime = DateTime.Now.ToString();
                    componentDetail.userRegister = userName;
                    componentDetail._id = mID;
                    componentDetail.id = mID.ToString();
                    componentDetail.subProjectID = newSubProjectID;
                    mongoDatabase.GetCollection<ComponentDetailsModel>("ComponentDetailsModel").InsertOne(componentDetail);
                }
                //
                var filterComponnt = Builders<Components>.Filter.Eq("subProjectID", subProjectID);
                List<Components> componentsInfo = mongoDatabase.GetCollection<Components>("Components").Find(filterComponnt).ToList();
                foreach (var item in componentsInfo)
                {
                    ObjectId mID = ObjectId.GenerateNewId();
                    item.creationTime = DateTime.Now.ToString();
                    item.userRegister = userName;
                    item._id = mID;
                    item.id = mID.ToString();
                    item.subProjectID = newSubProjectID;
                    mongoDatabase.GetCollection<Components>("Components").InsertOne(item);
                }
                //
                var filterDataSources = Builders<DataSources>.Filter.Where(p => p.subProjectID == subProjectID);
                List<DataSources> mList = mongoDatabase.GetCollection<DataSources>("DataSources").Find(filterDataSources).ToList();
                //
                foreach (var item in mList)
                {
                    ObjectId mID = ObjectId.GenerateNewId();
                    item._id = mID;
                    item.id = mID.ToString();
                    item.subProjectID = newSubProjectID;
                    mongoDatabase.GetCollection<DataSources>("DataSources").InsertOne(item);
                }
                AppSetting.saveUserLog(userName.ToLower(), "Project", "Duplicate Sub Project", subProjectID);

            }
            catch (Exception)
            {
                throw;
            }
            return newSubProjectID;
        }
        //Get recent sub projects for lef menu bar
        [Route("recentSubProject")]
        [HttpGet]
        public List<SubProjects> recentSubProject()
        {
            mongoDatabase = GetMongoDatabase();
            List<SubProjects> resultList = new List<SubProjects>();
            try
            {
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                //

                var collection = mongoDatabase.GetCollection<SubProjects>("SubProjects").AsQueryable().ToList();
                resultList = collection.Where(p => p.userCreator.ToLower() == userName).OrderByDescending(p => DateTime.Parse(p.createDate)).Take(5).ToList();
            }
            catch (Exception)
            {
            }
            return resultList;
        }
        //Get shared project for displaying in left menu bar
        [Route("getSharedProjectsForMenu")]
        [HttpGet]
        public List<SharedProjects> getSharedProjectsForMenu()
        {
            List<SharedProjects> resutList = new List<SharedProjects>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                var filter = Builders<SharedProjects>.Filter.Eq("sharedToUserName", userName);
                resutList = mongoDatabase.GetCollection<SharedProjects>("SharedProjects").Find(filter).ToList();
            }
            catch (Exception)
            {
            }
            return resutList;
        }
        //Change project info for specific sub project
        [Route("TransferSubProject/{subProjectID}/{projectID}")]
        [HttpGet("{subProjectID}/{projectID}")]
        public void TransferSubProject(string subProjectID, string projectID)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                var filter = Builders<SubProjects>.Filter.Eq("id", subProjectID);
                var updatestatement = Builders<SubProjects>.Update.Set("projectID", projectID);
                var result = mongoDatabase.GetCollection<SubProjects>("SubProjects").UpdateMany(filter, updatestatement);
                AppSetting.saveUserLog(userName.ToLower(), "Project", "Trasnfer sub Project", subProjectID);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //Approve sb project that shared for another user
        [Route("approveSharedProject/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public void approveSharedProject(string subProjectID)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                var filter = Builders<SharedProjects>.Filter.Where(p => p.sharedToUserName == userName && p.subProjectID == subProjectID);
                var updatestatement = Builders<SharedProjects>.Update.Set("sharedProjectStatus", "Approved");
                var result = mongoDatabase.GetCollection<SharedProjects>("SharedProjects").UpdateMany(filter, updatestatement);
                //
                var filterPr = Builders<SubProjects>.Filter.Where(p => p.id == subProjectID);
                var updatestatementPr = Builders<SubProjects>.Update.Set("workFlowStatus", "Approved");
                var resultPr = mongoDatabase.GetCollection<SubProjects>("SubProjects").UpdateMany(filterPr, updatestatementPr);
            }
            catch (Exception M)
            {
                throw;
            }
        }
        //Check permission of current shared sub project.
        [Route("checkPermissionSharedProject/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public List<SharedProjects> checkPermissionSharedProject(string subProjectID)
        {
            List<SharedProjects> resultList = new List<SharedProjects>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                string userName = Request.Cookies["userName"];
                userName = userName.ToLower();
                var filter = Builders<SharedProjects>.Filter.Where(p => p.sharedToUserName == userName && p.subProjectID == subProjectID);
                resultList = mongoDatabase.GetCollection<SharedProjects>("SharedProjects").Find(filter).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }
    }
    public class projectAndSubProjectHierarchy
    {
        public string projectID { get; set; }
        public string projectName { get; set; }
        public List<SubProjects> subProjectsList { get; set; }
    }
}
