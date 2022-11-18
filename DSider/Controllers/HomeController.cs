using DSider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private readonly AppSettings AppSetting;
        private IMongoDatabase mongoDatabase;
        //Initialize MongoDB Connections From appsettings.json
        public IMongoDatabase GetMongoDatabase()
        {
            string ConnectionString = appSettings.Value.ConnectionString;
            string DbName = appSettings.Value.DatabaseName;
            var mongoClient = new MongoClient(ConnectionString);
            return mongoClient.GetDatabase(DbName);
        }

        public HomeController(IOptions<DatabaseSettings> app)
        {
            appSettings = app;
            AppSetting = new AppSettings(appSettings);
        }
        
        //Index Controller
        public IActionResult Index()
        {
            string userName = Request.Cookies["userName"];
            users mUser = AppSetting.checkUserValidation(userName);
            if (mUser != null && mUser.userName != null)
            {
                AppSetting.saveUserLog(userName.ToLower(), "Project", "View", "");
                return View();
            }
            else
                return Redirect("~/Account/Login");
        }

        public IActionResult Register()
        {
            return View();
        }
        //Green hydrogen dashboard page Controller
        public IActionResult Dashboard()
        {
            string userName = Request.Cookies["userName"];
            users mUser = AppSetting.checkUserValidation(userName);
            if (mUser != null && mUser.userName != null)
            {
                AppSetting.saveUserLog(userName.ToLower(), "Dashboard", "View", "");
                return View();
            }
            else
                return Redirect("~/Account/Login");
        }
        //Carbon mitigation dashboard page controller
        public IActionResult DashboardMitigation()
        {
            string userName = Request.Cookies["userName"];
            users mUser = AppSetting.checkUserValidation(userName);
            if (mUser != null && mUser.userName != null)
            {
                AppSetting.saveUserLog(userName.ToLower(), "DashboardMitigation", "View", "");
                return View();
            }
            else
                return Redirect("~/Account/Login");
        }
        //Simulation (model design) page controller
        public ActionResult Simulation(string project, string name, string folder, string type)
        {
            string userName = Request.Cookies["userName"];
            users mUser = AppSetting.checkUserValidation(userName);
            if (mUser != null && mUser.userName != null)
            {
                AppSetting.saveUserLog(userName.ToLower(), "Simulation", "View", "");
                ViewData["subProjectID"] = project;
                ViewData["modelType"] = type;
                return View();
            }
            else
                return Redirect("~/Account/Login");
        }
        //Projects page controller
        public ActionResult Projects()
        {
            string userName = Request.Cookies["userName"];
            users mUser = AppSetting.checkUserValidation(userName);
            if (mUser != null && mUser.userName != null)
            {
                AppSetting.saveUserLog(userName.ToLower(), "Projects", "View", "");
                return View();
            }
            else
                return Redirect("~/Account/Login");
        }
       //When click on export json file this event will be fire
        public IActionResult downloadExportJSON(string id)
        {
            try
            {
                string userName = Request.Cookies["userName"];
                AppSetting.saveUserLog(userName.ToLower(), "downloadExportJSON", "downloadExportJSON", "");
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<SubProjects>.Filter.Eq("id", id);
                var collection = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filter).ToList();
                string jsonContent = collection[0].exportJSON;
                MemoryStream memoryStream = new MemoryStream();
                TextWriter tw = new StreamWriter(memoryStream);

                tw.WriteLine(jsonContent);
                tw.Flush();

                var length = memoryStream.Length;
                tw.Close();
                var toWrite = new byte[length];
                Array.Copy(memoryStream.GetBuffer(), 0, toWrite, 0, length);
                return File(toWrite, "text/plain", collection[0].name + ".json");

            }
            catch (Exception)
            {
            }
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
