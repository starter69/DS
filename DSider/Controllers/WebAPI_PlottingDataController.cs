using DSider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPI_PlottingDataController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        AppSettings AppSetting;
        //Initialize MongoDB Connections From appsettings.json
        public WebAPI_PlottingDataController(IOptions<DatabaseSettings> app)
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
        //Get plotting info from PlottingData Collection for green hydrogen dashboard.
        //PlottingData collection is filled from csv file that cretated from third party software.
        [Route("getPlottingData")]
        [HttpPost]
        public List<PlottingDataClass> getPlottingData([FromBody] string[] subProjectID)
        {
            List<PlottingDataClass> resultList = new List<PlottingDataClass>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                foreach (var item in subProjectID)
                {
                    var filterSubProject = Builders<SubProjects>.Filter.Where(p => p.id == item);
                    var collectionSub = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filterSubProject).ToList();
                    //
                    var filter = Builders<PlottingData>.Filter.Where(p => p.subProjectID == (item));
                    List<PlottingData> mList = mongoDatabase.GetCollection<PlottingData>("PlottingData").Find(filter).ToList();
                    resultList.Add(new PlottingDataClass()
                    {
                        mData = mList,
                        subProjectID = item,
                        subProjectName = collectionSub[0].name
                    });
                }

            }
            catch (Exception M)
            {
                string t = M.Message;
            }
            return resultList;
        }
        //Get plotting data for green hydrogen dashboard1
        [Route("getPlottingDataForDash1/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public List<PlottingData> getPlottingDataForDash1(string subProjectID)
        {
            List<PlottingData> resultList = new List<PlottingData>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                //var filter = Builders<PlottingData>.Filter.Where(p => p.subProjectID == subProjectID && (p.timestep) <= "2160");
                var filter = Builders<PlottingData>.Filter.Eq("subProjectID", subProjectID);
                resultList = mongoDatabase.GetCollection<PlottingData>("PlottingData").Find(filter).ToList();//.Take(2160).ToList();
                resultList = resultList.OrderBy(p => float.Parse(p.timestep)).Take(2160).ToList();//.Take(2160).ToList();
            }
            catch (Exception M)
            {
                string t = M.Message;
            }
            return resultList;
        }
        //Get Statistics for display in green hydrogen dashboard
        [Route("getDashboardStatistics")]
        [HttpPost]
        public List<DashboardStatisticsClass> getDashboardStatistics([FromBody] string[] subProjectID)
        {
            List<DashboardStatisticsClass> resultList = new List<DashboardStatisticsClass>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                foreach (var item in subProjectID)
                {
                    if (item != "")
                    {
                        var filterSubProject = Builders<SubProjects>.Filter.Where(p => p.id == item);
                        var collectionSub = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filterSubProject).ToList();
                        //
                        var filter = Builders<DashboardStatistics>.Filter.Where(p => p.subProjectID == (item));
                        List<DashboardStatistics> mList = mongoDatabase.GetCollection<DashboardStatistics>("DashboardStatistics").Find(filter).ToList();
                        if (mList.Count > 0)
                            resultList.Add(new DashboardStatisticsClass()
                            {
                                mData = mList[0],
                                subProjectID = item,
                                subProjectName = collectionSub[0].name
                            });
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return resultList;
        }
        //Get plotting data to display in Carbon mitigation dashboard.
        //This data extract from Summation Model collection
        [Route("carbonMitigaionDashboardPlot/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public List<carbonMitigationPlotClass> carbonMitigaionDashboardPlot(string subProjectID)
        {
            List<carbonMitigationPlotClass> resultList = new List<carbonMitigationPlotClass>();
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filter = Builders<SummationModel>.Filter.Where(p => p.subProjectID == subProjectID);
                List<SummationModel> modelData = mongoDatabase.GetCollection<SummationModel>("SummationModel").Find(filter).ToList();
                var result = modelData
                             .GroupBy(p => new { p.Year, p.dataType })
                            .Select(g => new
                            {
                                Sum_Scope_1_emission = g.Sum(e => e.Scope_1_emission),
                                Sum_Scope_2_consumption = g.Sum(e => e.Scope_2_consumption),
                                Sum_Scope_3 = g.Sum(e => e.Scope_3),
                                Year = g.Key.Year,
                                Sum_OpEx = g.Sum(e => e.OpEx),
                                Sum_CapEx = g.Sum(e => e.CapEx),
                                dataType = g.Key.dataType,
                            }).ToList();
                //
                var resultREfinery = modelData
                            .GroupBy(p => new { p.Year, p.dataType, p.parentNodeName, p.levelNumber })
                           .Select(g => new
                           {
                               Sum_Scope_1_emission = g.Sum(e => e.Scope_1_emission),
                               Sum_Scope_2_consumption = g.Sum(e => e.Scope_2_consumption),
                               Sum_Scope_3 = g.Sum(e => e.Scope_3),
                               Year = g.Key.Year,
                               Sum_OpEx = g.Sum(e => e.OpEx),
                               Sum_CapEx = g.Sum(e => e.CapEx),
                               dataType = g.Key.dataType,
                               nodeName = g.Key.parentNodeName,
                               levelNumber = g.Key.levelNumber
                           }).ToList();

                resultList.Add(new carbonMitigationPlotClass()
                {
                    capexBaseLine = result.Where(p => p.dataType == "BaseLine").Select(p => p.Sum_CapEx).ToList(),
                    reductionBaseLine = result.Where(p => p.dataType == "BaseLine").Select(p => p.Sum_Scope_1_emission + p.Sum_Scope_2_consumption + p.Sum_Scope_3).ToList(),
                    reductionAchieve = result.Where(p => p.dataType == "Achieve").Select(p => p.Sum_Scope_1_emission + p.Sum_Scope_2_consumption +
                    p.Sum_Scope_3).ToList(),
                    capexDeploy = result.Where(p => p.dataType == "Achieve").Select(p => p.Sum_CapEx).ToList(),
                    xAxis = result.Where(p => p.dataType == "BaseLine").Select(p => p.Year).ToList(),
                    capexBudget = result.Where(p => p.dataType == "Budget").Select(p => p.Sum_CapEx).ToList(),
                    reductionTarget = result.Where(p => p.dataType == "EmissionTargetReduction").Select(p => p.Sum_Scope_1_emission + p.Sum_Scope_2_consumption + p.Sum_Scope_3).ToList(),
                    Scope1BaseLine = result.Where(p => p.dataType == "BaseLine").Select(p => p.Sum_Scope_1_emission).ToList(),
                    Scope2BaseLine = result.Where(p => p.dataType == "BaseLine").Select(p => p.Sum_Scope_2_consumption).ToList(),
                    Scope3BaseLine = result.Where(p => p.dataType == "BaseLine").Select(p => p.Sum_Scope_3).ToList(),
                    Scope1Achieve = result.Where(p => p.dataType == "Achieve").Select(p => p.Sum_Scope_1_emission).ToList(),
                    Scope2Achieve = result.Where(p => p.dataType == "Achieve").Select(p => p.Sum_Scope_2_consumption).ToList(),
                    Scope3Achieve = result.Where(p => p.dataType == "Achieve").Select(p => p.Sum_Scope_3).ToList(),
                    Scope1TargetReduction = result.Where(p => p.dataType == "EmissionTargetReduction").Select(p => p.Sum_Scope_1_emission).ToList(),
                    Scope2TargetReduction = result.Where(p => p.dataType == "EmissionTargetReduction").Select(p => p.Sum_Scope_2_consumption).ToList(),
                    Scope3TargetReduction = result.Where(p => p.dataType == "EmissionTargetReduction").Select(p => p.Sum_Scope_3).ToList(),
                });
                //add refinery values to Achieve
                var finalResult = resultREfinery.Where(p => p.nodeName == "Refinery" && p.dataType == "Achieve").ToList();
                //
                var resultLevel0Level1 = resultREfinery.Where(p => (p.levelNumber == "0" || p.levelNumber == "1") && p.dataType == "BaseLine").ToList();
                int counter = 0;
                foreach (var item in resultList[0].xAxis)
                {
                    var refineryReductions = finalResult.Where(p => p.Year == item).ToList();
                    double sumCapex = resultLevel0Level1.Where(p => p.Year == item).Sum(p => p.Sum_CapEx);
                    double sumScope1ForREduction = resultLevel0Level1.Where(p => p.Year == item).Sum(p => p.Sum_Scope_1_emission);
                    double sumScope2ForREduction = resultLevel0Level1.Where(p => p.Year == item).Sum(p => p.Sum_Scope_2_consumption);
                    double sumScope3ForREduction = resultLevel0Level1.Where(p => p.Year == item).Sum(p => p.Sum_Scope_3);
                    //
                    if (refineryReductions.Count > 0)
                    {
                        resultList[0].reductionAchieve[counter] = refineryReductions[0].Sum_Scope_1_emission + refineryReductions[0].Sum_Scope_2_consumption +
                           refineryReductions[0].Sum_Scope_3 + sumScope1ForREduction + sumScope2ForREduction + sumScope3ForREduction;
                        //
                        resultList[0].Scope1Achieve[counter] += refineryReductions[0].Sum_Scope_1_emission + sumScope1ForREduction;
                        resultList[0].Scope2Achieve[counter] += refineryReductions[0].Sum_Scope_2_consumption + sumScope2ForREduction;
                        resultList[0].Scope3Achieve[counter] += refineryReductions[0].Sum_Scope_3 + sumScope3ForREduction;
                        //
                        resultList[0].Scope1TargetReduction[counter] += refineryReductions[0].Sum_Scope_1_emission + sumScope1ForREduction;
                        resultList[0].Scope2TargetReduction[counter] += refineryReductions[0].Sum_Scope_2_consumption + sumScope2ForREduction;
                        resultList[0].Scope3TargetReduction[counter] += refineryReductions[0].Sum_Scope_3 + sumScope3ForREduction;
                    }

                    //
                    resultList[0].capexDeploy[counter] += sumCapex;
                    resultList[0].capexBudget[counter] += sumCapex;
                    resultList[0].reductionTarget[counter] += sumScope1ForREduction + sumScope2ForREduction + sumScope3ForREduction;
                    counter++;
                }

                string t = "Amin";
            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }
    }
    public class carbonMitigationPlotClass
    {
        public List<int> xAxis { get; set; }
        public List<double> reductionAchieve { get; set; }
        public List<double> reductionTarget { get; set; }
        public List<double> reductionBaseLine { get; set; }
        public List<double> capexBudget { get; set; }
        public List<double> capexDeploy { get; set; }
        public List<double> capexBaseLine { get; set; }
        public List<double> Scope1BaseLine { get; set; }
        public List<double> Scope2BaseLine { get; set; }
        public List<double> Scope3BaseLine { get; set; }
        public List<double> Scope1Achieve { get; set; }
        public List<double> Scope2Achieve { get; set; }
        public List<double> Scope3Achieve { get; set; }
        public List<double> Scope1TargetReduction { get; set; }
        public List<double> Scope2TargetReduction { get; set; }
        public List<double> Scope3TargetReduction { get; set; }
    }
    public class DashboardStatisticsClass
    {
        public DashboardStatistics mData { get; set; }
        public string subProjectID { get; set; }
        public string subProjectName { get; set; }
    }
    public class PlottingDataClass
    {
        public List<PlottingData> mData { get; set; }
        public string subProjectID { get; set; }
        public string subProjectName { get; set; }
    }
}
