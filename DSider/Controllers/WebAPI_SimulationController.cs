using DSider.Controllers.SimulationComputation;
using DSider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPI_SimulationController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        private readonly IMemoryCache memoryCache;
        //Initialize MongoDB Connections From appsettings.json
        public WebAPI_SimulationController(IOptions<DatabaseSettings> app, IMemoryCache memoryCache)
        {
            appSettings = app;
            this.memoryCache = memoryCache;
        }
        public IMongoDatabase GetMongoDatabase()
        {
            string ConnectionString = appSettings.Value.ConnectionString;
            string DbName = appSettings.Value.DatabaseName;
            var mongoClient = new MongoClient(ConnectionString);
            return mongoClient.GetDatabase(DbName);
        }
        #region
        //[Route("doSimulation/{subProjectID}")]
        //[HttpGet("{subProjectID}")]
        //public async Task doSimulation(string subProjectID)
        //{
        //    return;
        //    mongoDatabase = GetMongoDatabase();
        //    DataSourcesController dtController = new DataSourcesController(memoryCache);
        //    List<RenewableClass> renewableList = new List<RenewableClass>();
        //    List<BatteryClass> batteryList = new List<BatteryClass>();
        //    List<ElectrolyserClass> electrolyserList = new List<ElectrolyserClass>();

        //    try
        //    {
        //        //
        //        ComputeBattery mBatteryController = new ComputeBattery();
        //        ComputeRenewablePower mRenewableController = new ComputeRenewablePower();
        //        ComputeElectrolyser mElectrolyserController = new ComputeElectrolyser();
        //        //
        //        var filterDataSources = Builders<DataSources>.Filter.Eq("subProjectID", subProjectID);
        //        List<DataSources> allDataSourceinThisProject = mongoDatabase.GetCollection<DataSources>("DataSources").Find(filterDataSources).ToList();
        //        //
        //        List<ComponentProperties> allProperties = mongoDatabase.GetCollection<ComponentProperties>("ComponentProperties").AsQueryable().ToList();
        //        //
        //        var filter = Builders<Components>.Filter.Eq("subProjectID", subProjectID);
        //        List<Components> subprojectComponents = mongoDatabase.GetCollection<Components>("Components").Find(filter).ToList();
        //        var groupedComponents = subprojectComponents
        //                    .GroupBy(u => u.type)
        //                    .Select(grp => grp.ToList())
        //                    .ToList();
        //        //
        //        foreach (var itemGrouped in groupedComponents)
        //        {
        //            for (int i = 0; i < itemGrouped.Count; i++)
        //            {
        //                if (itemGrouped[i].type == "turbine" || itemGrouped[i].type == "Solar")
        //                {
        //                    //get LF DataSource
        //                    List<ComponentProperty> LfComponent = itemGrouped[i].Propertiese.Where(p => p.name == "LF").ToList();
        //                    if (LfComponent.Count > 0)
        //                    {
        //                        string dataSourceName = LfComponent[0].value.Split('|')[0];
        //                        string dataSourceColoumn = LfComponent[0].value.Split('|')[1];
        //                        List<ComponentProperty> otherProperties = itemGrouped[i].Propertiese.Where(p => p.name == "pwrRECap").ToList();
        //                        string pwrRECap = otherProperties.Count > 0 ? otherProperties[0].value : "0";
        //                        otherProperties = itemGrouped[i].Propertiese.Where(p => p.name == "takeOfPercent").ToList();
        //                        string takeOfPercent = otherProperties.Count > 0 ? otherProperties[0].value : "0";
        //                        otherProperties = itemGrouped[i].Propertiese.Where(p => p.name == "TransLoss").ToList();
        //                        string TransLoss = otherProperties.Count > 0 ? otherProperties[0].value : "0";
        //                        //
        //                        DataSources mDataSourceInfo = allDataSourceinThisProject.Where(p => p.name.TrimEnd().ToLower() == dataSourceName.TrimEnd().ToLower()).Single();
        //                        DataTable csvDataTable = await dtController.getCSVFileContent(mDataSourceInfo.filePath, mDataSourceInfo.columnsList, dataSourceColoumn,
        //                            (mDataSourceInfo.name + subProjectID));
        //                        //
        //                        List<RenewableClass> mList = mRenewableController.doCompute(csvDataTable, dataSourceColoumn, pwrRECap, takeOfPercent, TransLoss,
        //                            itemGrouped[i].Propertiese.Where(p => p.name == "Name").Single().value);
        //                        renewableList.AddRange(mList);
        //                    }
        //                }
        //                else if (itemGrouped[i].type == "electricityLB")
        //                {
        //                    string elMinLoad = "0";
        //                    string electrolyzerCapacity = "0";
        //                    var electrolyzerInfo = subprojectComponents.Where(p => p.type == "electrolyzer").ToList();
        //                    if (electrolyzerInfo.Count > 0)
        //                    {
        //                        //10%
        //                        elMinLoad = electrolyzerInfo[0].Propertiese.Where(p => p.name.Contains("ElMinLoad")).Single().value;
        //                        //Capacity
        //                        electrolyzerCapacity = electrolyzerInfo[0].Propertiese.Where(p => p.name.Contains("ElCapStart")).Single().value;
        //                        float minDemandElctrolyzer = float.Parse(electrolyzerCapacity) * (float.Parse(elMinLoad) / 100);
        //                        //
        //                        var renewableSummry = renewableList.GroupBy(p => p.dateTime).Select(x =>
        //                           new RenewableClass
        //                           {
        //                               dateTime = x.Key,
        //                               pwrREOut = x.Sum(y => y.pwrREOut),
        //                               electrolyzerDemand10Percent = minDemandElctrolyzer - x.Sum(y => y.pwrREOut)
        //                           }
        //                        );
        //                        //
        //                        var batteryInfo = subprojectComponents.Where(p => p.type == "battery").ToList();
        //                        string capStart = batteryInfo[0].Propertiese.Where(p => p.name.Contains("BatCapStart")).Single().value;
        //                        string batCapDegrade = batteryInfo[0].Propertiese.Where(p => p.name.Contains("BatCapDegrade")).Single().value;
        //                        string pwrBatStoreStart = batteryInfo[0].Propertiese.Where(p => p.name.Contains("PwrBatStoreStart")).Single().value;
        //                        string LossBatIn = batteryInfo[0].Propertiese.Where(p => p.name.Contains("LossBatIn")).Single().value;
        //                        string LossBatStore = batteryInfo[0].Propertiese.Where(p => p.name.Contains("LossBatStore")).Single().value;
        //                        string LossBatOut = batteryInfo[0].Propertiese.Where(p => p.name.Contains("LossBatOut")).Single().value;
        //                        string MaxBatDCharge = batteryInfo[0].Propertiese.Where(p => p.name.Contains("MaxBatDCharge")).Single().value;
        //                        string MaxBatCharge = batteryInfo[0].Propertiese.Where(p => p.name.Contains("MaxBatCharge")).Single().value;
        //                        //
        //                        //Electrolyzer
        //                        string elCapStart = electrolyzerInfo[0].Propertiese.Where(p => p.name.Contains("ElCapStart")).Single().value;
        //                        string elDegrade = electrolyzerInfo[0].Propertiese.Where(p => p.name.Contains("ElDegrade")).Single().value;
        //                        string convert = electrolyzerInfo[0].Propertiese.Where(p => p.name.Contains("Convert")).Single().value;
        //                        string elecEff = electrolyzerInfo[0].Propertiese.Where(p => p.name.Contains("Eff")).Single().value;
        //                        foreach (var item in renewableSummry)
        //                        {
        //                            if (item.electrolyzerDemand10Percent >= 0 && float.Parse(electrolyzerCapacity) <= item.pwrREOut)
        //                            {
        //                                //Send to Electrolyzer
        //                                string PwrREDel = item.pwrREOut.ToString();
        //                                string PwrBatIn = "0";
        //                                string PwrBatOut = "0";
        //                                string PGrayElIn = "0";
        //                                string PwrREAvail = item.pwrREOut.ToString();
        //                                electrolyserList.Add(mElectrolyserController.doCompute(item, PwrREDel, PwrBatIn, PwrBatOut, PwrREAvail,
        //                                   elCapStart, elDegrade, convert, elecEff));
        //                                //for battery
        //                                //do PwrBatStore (t) = PwrBatStore (t-1) * (1-LossBatStore)
        //                            }
        //                            else
        //                            {
        //                                //Call Battery Modules
        //                                //Recieve Battery Available
        //                                string lastBatCap = batteryList.Count == 0 ? capStart : batteryList[batteryList.Count - 1].BatCap.ToString();
        //                                string PwrBatStore = batteryList.Count == 0 ? pwrBatStoreStart : batteryList[batteryList.Count - 1].PwrBatStore.ToString();
        //                                string PwrBatIn = item.pwrREOut.ToString();
        //                                string PwrBatOut = "0";
        //                                //
        //                                if (item.electrolyzerDemand10Percent >= 0)
        //                                {
        //                                    if (float.Parse(PwrBatStore) >= item.electrolyzerDemand10Percent)
        //                                        PwrBatOut = item.electrolyzerDemand10Percent.ToString();
        //                                    //else 
        //                                    //PGrayElIn = item.electrolyzerDemand10Percent - PwrBatStore;
        //                                    //وقتی میزان درخواستی از موجودی باتری بیشتر شود تفاضل از گرید گری گرفته می شود.
        //                                }
        //                                else if (float.Parse(electrolyzerCapacity) <= item.pwrREOut)
        //                                {
        //                                    PwrBatOut = "0";
        //                                    //PGrayElIn = 0
        //                                    PwrBatIn = Math.Min((item.pwrREOut - float.Parse(electrolyzerCapacity)),
        //                                        float.Parse(MaxBatCharge) * float.Parse(lastBatCap)).ToString();
        //                                }
        //                                batteryList.Add(mBatteryController.doCompute(item, capStart, batCapDegrade, lastBatCap, PwrBatStore, PwrBatIn, LossBatIn,
        //                                    LossBatStore, LossBatOut, PwrBatOut, MaxBatDCharge));
        //                                //
        //                                //call Electrolyser Again
        //                                //
        //                                string PwrREDel = (item.pwrREOut + batteryList[batteryList.Count].PwrBatAvail).ToString(); //+ Gray
        //                                string PwrREAvail = PwrREDel;
        //                                electrolyserList.Add(mElectrolyserController.doCompute(item, PwrREDel, PwrBatIn, PwrBatOut, PwrREAvail,
        //                                  elCapStart, elDegrade, convert, elecEff));
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception M)
        //    {
        //        string t = M.Message;
        //        throw;
        //    }
        //}
        //public void callBatteryModule()
        //{
        //    //
        //}
        //
        #endregion
        //Compute Summation for carbon mitigation
        //When click on simulation button and the type of model is carbon mitigation this event will be fire
        [Route("computeSummation/{subProjectID}")]
        [HttpGet("{subProjectID}")]
        public async Task computeSummation(string subProjectID)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                var filterDEleteSummationModel = Builders<SummationModel>.Filter.Eq("subProjectID", subProjectID);
                var result = mongoDatabase.GetCollection<SummationModel>("SummationModel").DeleteMany(filterDEleteSummationModel);
                string dataType = "";
                bool isBudget = false;
                bool isEmissionReductionTarget = false;
                //
                DataSourcesController dtController = new DataSourcesController(memoryCache);
                List<SummationModel> summationListLevel2 = new List<SummationModel>();
                List<SummationModel> summationListLevel1 = new List<SummationModel>();
                List<SummationModel> summationListLevel0 = new List<SummationModel>();

                var filterDataSources = Builders<DataSources>.Filter.Eq("subProjectID", subProjectID);
                List<DataSources> allDataSourceinThisProject = mongoDatabase.GetCollection<DataSources>("DataSources").Find(filterDataSources).ToList();
                #region Level2
                var filterLevel2 = Builders<ComponentDetailsModel>.Filter.Where(p => p.subProjectID == subProjectID && p.levelNumber == "2");
                List<ComponentDetailsModel> level2Details = mongoDatabase.GetCollection<ComponentDetailsModel>("ComponentDetailsModel").Find(filterLevel2).ToList();
                List<componentsInfoForSummation> mComponentsInfoForSummation = new List<componentsInfoForSummation>();
                DataTable totalDataTableLevel2 = new DataTable();
                DataTable totalDataTableLevel2Achieve = new DataTable();
                DataTable totalDataTableLevel2Budget = new DataTable();
                DataTable totalDataTableLevel2TargetReduction = new DataTable();

                totalDataTableLevel2.Columns.Add("Year", typeof(int));
                totalDataTableLevel2.Columns.Add("Scope_2_consumption", typeof(double));
                totalDataTableLevel2.Columns.Add("Scope_1_emission", typeof(double));
                totalDataTableLevel2.Columns.Add("Scope_3", typeof(double));
                totalDataTableLevel2.Columns.Add("OpEx", typeof(double));
                totalDataTableLevel2.Columns.Add("CapEx", typeof(double));
                //
                totalDataTableLevel2Achieve.Columns.Add("Year", typeof(int));
                totalDataTableLevel2Achieve.Columns.Add("Scope_2_consumption", typeof(double));
                totalDataTableLevel2Achieve.Columns.Add("Scope_1_emission", typeof(double));
                totalDataTableLevel2Achieve.Columns.Add("Scope_3", typeof(double));
                totalDataTableLevel2Achieve.Columns.Add("OpEx", typeof(double));
                totalDataTableLevel2Achieve.Columns.Add("CapEx", typeof(double));
                //
                totalDataTableLevel2Budget.Columns.Add("Year", typeof(int));
                totalDataTableLevel2Budget.Columns.Add("Scope_2_consumption", typeof(double));
                totalDataTableLevel2Budget.Columns.Add("Scope_1_emission", typeof(double));
                totalDataTableLevel2Budget.Columns.Add("Scope_3", typeof(double));
                totalDataTableLevel2Budget.Columns.Add("OpEx", typeof(double));
                totalDataTableLevel2Budget.Columns.Add("CapEx", typeof(double));
                //
                totalDataTableLevel2TargetReduction.Columns.Add("Year", typeof(int));
                totalDataTableLevel2TargetReduction.Columns.Add("Scope_2_consumption", typeof(double));
                totalDataTableLevel2TargetReduction.Columns.Add("Scope_1_emission", typeof(double));
                totalDataTableLevel2TargetReduction.Columns.Add("Scope_3", typeof(double));
                totalDataTableLevel2TargetReduction.Columns.Add("OpEx", typeof(double));
                totalDataTableLevel2TargetReduction.Columns.Add("CapEx", typeof(double));
                foreach (var itemLevel2 in level2Details)
                {
                    dynamic dynamicDataLevel3 = JObject.Parse(itemLevel2.exportJSON);
                    JObject dataLevel2 = dynamicDataLevel3.drawflow.Home.data;
                    foreach (var item in dataLevel2)
                    {
                        string id = item.Value["id"].ToString();
                        string name = "";
                        string className = item.Value["class"].ToString();
                        if (className == "Capex" || className == "TotalOut" || className == "Aggregator")
                            continue;
                        string inputs = item.Value["inputs"].ToString();
                        string outputs = item.Value["outputs"].ToString();
                        string data = item.Value["data"].ToString();
                        var dataObject = JObject.Parse(data);
                        JObject mData = dataObject;
                        List<myComponentProperty> currentProperty = new List<myComponentProperty>();
                        DataTable mainDataTable = new DataTable();
                        DataTable mainDataTableAchieve = new DataTable();
                        DataTable mainDataTableBudget = new DataTable();
                        DataTable mainDataTableTargetReduction = new DataTable();

                        foreach (var itemData in mData)
                        {
                            DataTable mDataTable = new DataTable();
                            if (itemData.Key == "Name")
                                name = itemData.Key;
                            else if (itemData.Value.ToString().Contains("|"))
                            {
                                string dataSourceName = itemData.Value.ToString().Split('|')[0];
                                string dataSourceColoumn = itemData.Value.ToString().Split('|')[1];
                                DataSources mDataSourceInfo = allDataSourceinThisProject.Where(p => p.name.TrimEnd().ToLower() == dataSourceName.TrimEnd().ToLower()).Single();
                                mDataTable = await dtController.getCSVFileContent(mDataSourceInfo.filePath, mDataSourceInfo.columnsList, dataSourceColoumn,
                                   (mDataSourceInfo.name + subProjectID));
                                //
                                if (itemData.Key.ToString().Contains("Achieve"))
                                    dataType = "Achieve";
                                else
                                    dataType = "BaseLine";
                                if (className == "Budget")
                                    isBudget = true;
                                else
                                    isBudget = false;
                                //
                                if (className == "EmissionReductionTarget")
                                    isEmissionReductionTarget = true;
                                else
                                    isEmissionReductionTarget = false;
                            }
                            if (mDataTable.Columns.Count > 0 && isBudget)
                                mainDataTableBudget = mDataTable;
                            else if (mDataTable.Columns.Count > 0 && isEmissionReductionTarget)
                                mainDataTableTargetReduction = mDataTable;
                            else if (mDataTable.Columns.Count > 0 & dataType == "BaseLine")
                                mainDataTable = mDataTable;
                            else if (mDataTable.Columns.Count > 0 & dataType == "Achieve")
                                mainDataTableAchieve = mDataTable;
                            //
                            currentProperty.Add(new myComponentProperty()
                            {
                                name = itemData.Key,
                                value = itemData.Value.ToString(),
                                mDataTable = mDataTable
                            });
                        }
                        mComponentsInfoForSummation.Add(new componentsInfoForSummation()
                        {
                            componentType = className,
                            id = id,
                            inputs = inputs,
                            levelNumber = "2",
                            name = name,
                            outputs = outputs,
                            property = currentProperty,
                            mainDataTable = mainDataTable
                        });
                        //Create big DataTable with all Data
                        if (mainDataTable.Columns.Count > 0)
                        {
                            string[] columnNames_Scope_2 = mainDataTable.Columns.Cast<DataColumn>()
                                            .Select(x => x.ColumnName)
                                            .Where(x => x.ToLower().Contains("scope 2") || x.ToLower().Contains("scope_2"))
                                            .ToArray();
                            string[] columnNames_Scope_1 = mainDataTable.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 1") || x.ToLower().Contains("scope_1"))
                                           .ToArray();
                            string[] columnNames_Scope_3 = mainDataTable.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 3") || x.ToLower().Contains("scope_3"))
                                           .ToArray();
                            string[] columnNames_OpEx = mainDataTable.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("opex"))
                                          .ToArray();
                            string[] columnNames_CapEx = mainDataTable.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("capex") || x.ToLower().Contains("budget"))
                                          .ToArray();
                            foreach (DataRow row in mainDataTable.Rows)
                            {
                                //Column name ha ro bayad bedash biyarim chon intori moshkel dare.
                                DataRow dr = totalDataTableLevel2.NewRow();
                                dr[0] = int.Parse(row[0].ToString().Trim().Replace('-', '0'));
                                if (columnNames_Scope_2.Length > 0)
                                    dr[1] = double.Parse(row[columnNames_Scope_2[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[1] = 0;
                                if (columnNames_Scope_1.Length > 0)
                                    dr[2] = double.Parse(row[columnNames_Scope_1[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[2] = 0;
                                if (columnNames_Scope_3.Length > 0)
                                    dr[3] = double.Parse(row[columnNames_Scope_3[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[3] = 0;
                                if (columnNames_OpEx.Length > 0)
                                    dr[4] = double.Parse(row[columnNames_OpEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[4] = 0;
                                if (columnNames_CapEx.Length > 0)
                                    dr[5] = double.Parse(row[columnNames_CapEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[5] = 0;
                                totalDataTableLevel2.Rows.Add(dr);
                            }
                        }
                        if (mainDataTableAchieve.Columns.Count > 0)
                        {
                            string[] columnNames_Scope_2 = mainDataTableAchieve.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 2") || x.ToLower().Contains("scope_2"))
                                           .ToArray();
                            string[] columnNames_Scope_1 = mainDataTableAchieve.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 1") || x.ToLower().Contains("scope_1"))
                                           .ToArray();
                            string[] columnNames_Scope_3 = mainDataTableAchieve.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 3") || x.ToLower().Contains("scope_3"))
                                           .ToArray();
                            string[] columnNames_OpEx = mainDataTableAchieve.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("opex"))
                                          .ToArray();
                            string[] columnNames_CapEx = mainDataTableAchieve.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("capex") || x.ToLower().Contains("budget"))
                                          .ToArray();
                            foreach (DataRow row in mainDataTableAchieve.Rows)
                            {
                                DataRow dr = totalDataTableLevel2Achieve.NewRow();
                                dr[0] = int.Parse(row[0].ToString().Trim().Replace('-', '0'));
                                if (columnNames_Scope_2.Length > 0)
                                    dr[1] = double.Parse(row[columnNames_Scope_2[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[1] = 0;
                                if (columnNames_Scope_1.Length > 0)
                                    dr[2] = double.Parse(row[columnNames_Scope_1[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[2] = 0;
                                if (columnNames_Scope_3.Length > 0)
                                    dr[3] = double.Parse(row[columnNames_Scope_3[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[3] = 0;
                                if (columnNames_OpEx.Length > 0)
                                    dr[4] = double.Parse(row[columnNames_OpEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[4] = 0;
                                if (columnNames_CapEx.Length > 0)
                                    dr[5] = double.Parse(row[columnNames_CapEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[5] = 0;
                                totalDataTableLevel2Achieve.Rows.Add(dr);
                            }
                        }
                        if (mainDataTableBudget.Columns.Count > 0)
                        {
                            string[] columnNames_Scope_2 = mainDataTableBudget.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 2") || x.ToLower().Contains("scope_2"))
                                           .ToArray();
                            string[] columnNames_Scope_1 = mainDataTableBudget.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 1") || x.ToLower().Contains("scope_1"))
                                           .ToArray();
                            string[] columnNames_Scope_3 = mainDataTableBudget.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 3") || x.ToLower().Contains("scope_3"))
                                           .ToArray();
                            string[] columnNames_OpEx = mainDataTableBudget.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("opex"))
                                          .ToArray();
                            string[] columnNames_CapEx = mainDataTableBudget.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("capex") || x.ToLower().Contains("budget"))
                                          .ToArray();
                            foreach (DataRow row in mainDataTableBudget.Rows)
                            {
                                DataRow dr = totalDataTableLevel2Budget.NewRow();
                                dr[0] = int.Parse(row[0].ToString().Trim().Replace('-', '0'));
                                if (columnNames_Scope_2.Length > 0)
                                    dr[1] = double.Parse(row[columnNames_Scope_2[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[1] = 0;
                                if (columnNames_Scope_1.Length > 0)
                                    dr[2] = double.Parse(row[columnNames_Scope_1[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[2] = 0;
                                if (columnNames_Scope_3.Length > 0)
                                    dr[3] = double.Parse(row[columnNames_Scope_3[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[3] = 0;
                                if (columnNames_OpEx.Length > 0)
                                    dr[4] = double.Parse(row[columnNames_OpEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[4] = 0;
                                if (columnNames_CapEx.Length > 0)
                                    dr[5] = double.Parse(row[columnNames_CapEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[5] = 0;
                                totalDataTableLevel2Budget.Rows.Add(dr);
                            }
                        }
                        if (mainDataTableTargetReduction.Columns.Count > 0)
                        {
                            string[] columnNames_Scope_2 = mainDataTableTargetReduction.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 2") || x.ToLower().Contains("scope_2") || x.ToLower().Contains("scope2"))
                                           .ToArray();
                            string[] columnNames_Scope_1 = mainDataTableTargetReduction.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 1") || x.ToLower().Contains("scope_1") || x.ToLower().Contains("scope1"))
                                           .ToArray();
                            string[] columnNames_Scope_3 = mainDataTableTargetReduction.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 3") || x.ToLower().Contains("scope_3") || x.ToLower().Contains("scope3"))
                                           .ToArray();
                            string[] columnNames_OpEx = mainDataTableTargetReduction.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("opex"))
                                          .ToArray();
                            string[] columnNames_CapEx = mainDataTableTargetReduction.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("capex") || x.ToLower().Contains("budget"))
                                          .ToArray();
                            foreach (DataRow row in mainDataTableTargetReduction.Rows)
                            {
                                DataRow dr = totalDataTableLevel2TargetReduction.NewRow();
                                dr[0] = int.Parse(row[0].ToString().Trim().Replace('-', '0'));
                                if (columnNames_Scope_2.Length > 0)
                                    dr[1] = double.Parse(row[columnNames_Scope_2[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[1] = 0;
                                if (columnNames_Scope_1.Length > 0)
                                    dr[2] = double.Parse(row[columnNames_Scope_1[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[2] = 0;
                                if (columnNames_Scope_3.Length > 0)
                                    dr[3] = double.Parse(row[columnNames_Scope_3[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[3] = 0;
                                if (columnNames_OpEx.Length > 0)
                                    dr[4] = double.Parse(row[columnNames_OpEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[4] = 0;
                                if (columnNames_CapEx.Length > 0)
                                    dr[5] = double.Parse(row[columnNames_CapEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[5] = 0;
                                totalDataTableLevel2TargetReduction.Rows.Add(dr);
                            }
                        }
                    }
                    //
                    if (totalDataTableLevel2.Rows.Count > 0)
                    {
                        var newDt = totalDataTableLevel2.AsEnumerable()
                            .GroupBy(r => r.Field<int>("Year"))
                            .Select(g =>
                        {
                            var row = totalDataTableLevel2.NewRow();
                            row["Year"] = g.Key;
                            row["Scope_2_consumption"] = g.Sum(r => r.Field<double>("Scope_2_consumption"));
                            row["Scope_1_emission"] = g.Sum(r => r.Field<double>("Scope_1_emission"));
                            row["Scope_3"] = g.Sum(r => r.Field<double>("Scope_3"));
                            row["OpEx"] = g.Sum(r => r.Field<double>("OpEx"));
                            row["CapEx"] = g.Sum(r => r.Field<double>("CapEx"));
                            return row;
                        }).CopyToDataTable();
                        summationListLevel2 = newDt.AsEnumerable().Select(m => new SummationModel()
                        {
                            Year = m.Field<int>("Year"),
                            Scope_1_emission = m.Field<double>("Scope_1_emission"),
                            Scope_2_consumption = m.Field<double>("Scope_2_consumption"),
                            Scope_3 = m.Field<double>("Scope_3"),
                            OpEx = m.Field<double>("OpEx"),
                            CapEx = m.Field<double>("CapEx"),
                            levelNumber = "2",
                            parentNodeID = itemLevel2.nodeID,
                            subProjectID = subProjectID,
                            parentNodeName = itemLevel2.nodeName,
                            dataType = "BaseLine"
                        }).ToList();
                        mongoDatabase.GetCollection<SummationModel>("SummationModel").InsertMany(summationListLevel2);
                    }
                    //
                    //Achieve 
                    //
                    if (totalDataTableLevel2Achieve.Rows.Count > 0)
                    {
                        var newDtAchieve = totalDataTableLevel2Achieve.AsEnumerable()
                         .GroupBy(r => r.Field<int>("Year"))
                         .Select(g =>
                         {
                             var row = totalDataTableLevel2Achieve.NewRow();
                             row["Year"] = g.Key;
                             row["Scope_2_consumption"] = g.Sum(r => r.Field<double>("Scope_2_consumption"));
                             row["Scope_1_emission"] = g.Sum(r => r.Field<double>("Scope_1_emission"));
                             row["Scope_3"] = g.Sum(r => r.Field<double>("Scope_3"));
                             row["OpEx"] = g.Sum(r => r.Field<double>("OpEx"));
                             row["CapEx"] = g.Sum(r => r.Field<double>("CapEx"));
                             return row;
                         }).CopyToDataTable();
                        summationListLevel2 = newDtAchieve.AsEnumerable().Select(m => new SummationModel()
                        {
                            Year = m.Field<int>("Year"),
                            Scope_1_emission = m.Field<double>("Scope_1_emission"),
                            Scope_2_consumption = m.Field<double>("Scope_2_consumption"),
                            Scope_3 = m.Field<double>("Scope_3"),
                            OpEx = m.Field<double>("OpEx"),
                            CapEx = m.Field<double>("CapEx"),
                            levelNumber = "2",
                            parentNodeID = itemLevel2.nodeID,
                            subProjectID = subProjectID,
                            parentNodeName = itemLevel2.nodeName,
                            dataType = "Achieve"
                        }).ToList();
                        mongoDatabase.GetCollection<SummationModel>("SummationModel").InsertMany(summationListLevel2);
                    }
                    //
                    //Budget 
                    //
                    if (totalDataTableLevel2Budget.Rows.Count > 0)
                    {
                        var newDtBudget = totalDataTableLevel2Budget.AsEnumerable()
                         .GroupBy(r => r.Field<int>("Year"))
                         .Select(g =>
                         {
                             var row = totalDataTableLevel2Budget.NewRow();
                             row["Year"] = g.Key;
                             row["Scope_2_consumption"] = g.Sum(r => r.Field<double>("Scope_2_consumption"));
                             row["Scope_1_emission"] = g.Sum(r => r.Field<double>("Scope_1_emission"));
                             row["Scope_3"] = g.Sum(r => r.Field<double>("Scope_3"));
                             row["OpEx"] = g.Sum(r => r.Field<double>("OpEx"));
                             row["CapEx"] = g.Sum(r => r.Field<double>("CapEx"));
                             return row;
                         }).CopyToDataTable();
                        summationListLevel2 = newDtBudget.AsEnumerable().Select(m => new SummationModel()
                        {
                            Year = m.Field<int>("Year"),
                            Scope_1_emission = m.Field<double>("Scope_1_emission"),
                            Scope_2_consumption = m.Field<double>("Scope_2_consumption"),
                            Scope_3 = m.Field<double>("Scope_3"),
                            OpEx = m.Field<double>("OpEx"),
                            CapEx = m.Field<double>("CapEx"),
                            levelNumber = "2",
                            parentNodeID = itemLevel2.nodeID,
                            subProjectID = subProjectID,
                            parentNodeName = itemLevel2.nodeName,
                            dataType = "Budget"
                        }).ToList();
                        mongoDatabase.GetCollection<SummationModel>("SummationModel").InsertMany(summationListLevel2);
                    }
                    //
                    //Taget Reduction 
                    //
                    if (totalDataTableLevel2TargetReduction.Rows.Count > 0)
                    {
                        var newDtTarget = totalDataTableLevel2TargetReduction.AsEnumerable()
                         .GroupBy(r => r.Field<int>("Year"))
                         .Select(g =>
                         {
                             var row = totalDataTableLevel2TargetReduction.NewRow();
                             row["Year"] = g.Key;
                             row["Scope_2_consumption"] = g.Sum(r => r.Field<double>("Scope_2_consumption"));
                             row["Scope_1_emission"] = g.Sum(r => r.Field<double>("Scope_1_emission"));
                             row["Scope_3"] = g.Sum(r => r.Field<double>("Scope_3"));
                             row["OpEx"] = g.Sum(r => r.Field<double>("OpEx"));
                             row["CapEx"] = g.Sum(r => r.Field<double>("CapEx"));
                             return row;
                         }).CopyToDataTable();
                        summationListLevel2 = newDtTarget.AsEnumerable().Select(m => new SummationModel()
                        {
                            Year = m.Field<int>("Year"),
                            Scope_1_emission = m.Field<double>("Scope_1_emission"),
                            Scope_2_consumption = m.Field<double>("Scope_2_consumption"),
                            Scope_3 = m.Field<double>("Scope_3"),
                            OpEx = m.Field<double>("OpEx"),
                            CapEx = m.Field<double>("CapEx"),
                            levelNumber = "2",
                            parentNodeID = itemLevel2.nodeID,
                            subProjectID = subProjectID,
                            parentNodeName = itemLevel2.nodeName,
                            dataType = "EmissionTargetReduction"
                        }).ToList();
                        mongoDatabase.GetCollection<SummationModel>("SummationModel").InsertMany(summationListLevel2);
                    }
                }
                isEmissionReductionTarget = isBudget = false;
                //
                #endregion
                //
                #region Level1
                var filterLevel1 = Builders<ComponentDetailsModel>.Filter.Where(p => p.subProjectID == subProjectID && p.levelNumber == "1");
                List<ComponentDetailsModel> level1Details = mongoDatabase.GetCollection<ComponentDetailsModel>("ComponentDetailsModel").Find(filterLevel1).ToList();
                mComponentsInfoForSummation = new List<componentsInfoForSummation>();
                DataTable totalDataTableLevel1 = new DataTable();
                totalDataTableLevel1.Columns.Add("Year", typeof(int));
                totalDataTableLevel1.Columns.Add("Scope_2_consumption", typeof(double));
                totalDataTableLevel1.Columns.Add("Scope_1_emission", typeof(double));
                totalDataTableLevel1.Columns.Add("Scope_3", typeof(double));
                totalDataTableLevel1.Columns.Add("OpEx", typeof(double));
                totalDataTableLevel1.Columns.Add("CapEx", typeof(double));
                foreach (var itemLevel1 in level1Details)
                {
                    dynamic dynamicDataLevel1 = JObject.Parse(itemLevel1.exportJSON);
                    JObject dataLevel1 = dynamicDataLevel1.drawflow.Home.data;
                    foreach (var item in dataLevel1)
                    {
                        string id = item.Value["id"].ToString();
                        string name = "";
                        string className = item.Value["class"].ToString();
                        if (className == "Budget" || className == "EmissionReductionTarget" || className == "Capex" || className == "TotalOut" || className == "Aggregator")
                            continue;
                        string inputs = item.Value["inputs"].ToString();
                        string outputs = item.Value["outputs"].ToString();
                        string data = item.Value["data"].ToString();
                        var dataObject = JObject.Parse(data);
                        JObject mData = dataObject;
                        List<myComponentProperty> currentProperty = new List<myComponentProperty>();
                        DataTable mainDataTable = new DataTable();
                        foreach (var itemData in mData)
                        {
                            DataTable mDataTable = new DataTable();
                            if (itemData.Key == "Name")
                                name = itemData.Key;
                            else if (itemData.Value.ToString().Contains("|"))
                            {
                                string dataSourceName = itemData.Value.ToString().Split('|')[0];
                                string dataSourceColoumn = itemData.Value.ToString().Split('|')[1];
                                DataSources mDataSourceInfo = allDataSourceinThisProject.Where(p => p.name.TrimEnd().ToLower() == dataSourceName.TrimEnd().ToLower()).Single();
                                mDataTable = await dtController.getCSVFileContent(mDataSourceInfo.filePath, mDataSourceInfo.columnsList, dataSourceColoumn,
                                   (mDataSourceInfo.name + subProjectID));
                            }
                            if (mDataTable.Columns.Count > 0)
                                mainDataTable = mDataTable;
                            currentProperty.Add(new myComponentProperty()
                            {
                                name = itemData.Key,
                                value = itemData.Value.ToString(),
                                mDataTable = mDataTable
                            });
                        }
                        mComponentsInfoForSummation.Add(new componentsInfoForSummation()
                        {
                            componentType = className,
                            id = id,
                            inputs = inputs,
                            levelNumber = "1",
                            name = name,
                            outputs = outputs,
                            property = currentProperty,
                            mainDataTable = mainDataTable
                        });
                        //Create big DataTable with all Data
                        if (mainDataTable.Columns.Count > 0)
                        {
                            string[] columnNames_Scope_2 = mainDataTable.Columns.Cast<DataColumn>()
                                            .Select(x => x.ColumnName)
                                            .Where(x => x.ToLower().Contains("scope 2") || x.ToLower().Contains("scope_2"))
                                            .ToArray();
                            string[] columnNames_Scope_1 = mainDataTable.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 1") || x.ToLower().Contains("scope_1"))
                                           .ToArray();
                            string[] columnNames_Scope_3 = mainDataTable.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 3") || x.ToLower().Contains("scope_3"))
                                           .ToArray();
                            string[] columnNames_OpEx = mainDataTable.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("opex"))
                                          .ToArray();
                            string[] columnNames_CapEx = mainDataTable.Columns.Cast<DataColumn>()
                                          .Select(x => x.ColumnName)
                                          .Where(x => x.ToLower().Contains("capex") || x.ToLower().Contains("budget"))
                                          .ToArray();
                            foreach (DataRow row in mainDataTable.Rows)
                            {
                                DataRow dr = totalDataTableLevel1.NewRow();
                                dr[0] = int.Parse(row[0].ToString().Trim().Replace('-', '0'));
                                if (columnNames_Scope_2.Length > 0)
                                    dr[1] = double.Parse(row[columnNames_Scope_2[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[1] = 0;
                                if (columnNames_Scope_1.Length > 0)
                                    dr[2] = double.Parse(row[columnNames_Scope_1[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[2] = 0;
                                if (columnNames_Scope_3.Length > 0)
                                    dr[3] = double.Parse(row[columnNames_Scope_3[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[3] = 0;
                                if (columnNames_OpEx.Length > 0)
                                    dr[4] = double.Parse(row[columnNames_OpEx[0]].ToString().Trim().Replace('-', '0') == "" ? "0" : row[columnNames_OpEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[4] = 0;
                                if (columnNames_CapEx.Length > 0)
                                    dr[5] = double.Parse(row[columnNames_CapEx[0]].ToString().Trim().Replace('-', '0') == "" ? "0" : row[columnNames_CapEx[0]].ToString().Trim().Replace('-', '0'));
                                else
                                    dr[5] = 0;
                                totalDataTableLevel1.Rows.Add(dr);
                            }
                        }
                    }
                    //
                    var newDtLevel1 = totalDataTableLevel1.AsEnumerable()
                         .GroupBy(r => r.Field<int>("Year"))
                         .Select(g =>
                         {
                             var row = totalDataTableLevel1.NewRow();
                             row["Year"] = g.Key;
                             row["Scope_2_consumption"] = g.Sum(r => r.Field<double>("Scope_2_consumption"));
                             row["Scope_1_emission"] = g.Sum(r => r.Field<double>("Scope_1_emission"));
                             row["Scope_3"] = g.Sum(r => r.Field<double>("Scope_3"));
                             row["OpEx"] = g.Sum(r => r.Field<double>("OpEx"));
                             row["CapEx"] = g.Sum(r => r.Field<double>("CapEx"));
                             return row;
                         }).CopyToDataTable();
                    summationListLevel1 = newDtLevel1.AsEnumerable().Select(m => new SummationModel()
                    {
                        Year = m.Field<int>("Year"),
                        Scope_1_emission = m.Field<double>("Scope_1_emission"),
                        Scope_2_consumption = m.Field<double>("Scope_2_consumption"),
                        Scope_3 = m.Field<double>("Scope_3"),
                        OpEx = m.Field<double>("OpEx"),
                        CapEx = m.Field<double>("CapEx"),
                        levelNumber = "1",
                        parentNodeID = itemLevel1.nodeID,
                        parentNodeName = itemLevel1.nodeName,
                        subProjectID = subProjectID,
                        dataType = "BaseLine"
                    }).ToList();
                    mongoDatabase.GetCollection<SummationModel>("SummationModel").InsertMany(summationListLevel1);
                }
                //
                #endregion
                //
                #region Level0
                var filterSubProject = Builders<SubProjects>.Filter.Eq("id", subProjectID);
                SubProjects level0SubProject = mongoDatabase.GetCollection<SubProjects>("SubProjects").Find(filterSubProject).FirstOrDefault();
                string json = level0SubProject.exportJSON;
                //
                DataTable totalDataTableLevel0 = new DataTable();
                totalDataTableLevel0.Columns.Add("Year", typeof(int));
                totalDataTableLevel0.Columns.Add("Scope_2_consumption", typeof(double));
                totalDataTableLevel0.Columns.Add("Scope_1_emission", typeof(double));
                totalDataTableLevel0.Columns.Add("Scope_3", typeof(double));
                totalDataTableLevel0.Columns.Add("OpEx", typeof(double));
                totalDataTableLevel0.Columns.Add("CapEx", typeof(double));
                dynamic dynamicData = JObject.Parse(json);
                JObject tabulatedData = dynamicData.drawflow.Home.data;
                foreach (var item in tabulatedData)
                {
                    string id = item.Value["id"].ToString();
                    string name = "";
                    string className = item.Value["class"].ToString();
                    if (className == "Budget" || className == "EmissionReductionTarget" || className == "Capex" || className == "TotalOut" || className == "Aggregator")
                        continue;
                    string inputs = item.Value["inputs"].ToString();
                    string outputs = item.Value["outputs"].ToString();
                    string data = item.Value["data"].ToString();
                    var dataObject = JObject.Parse(data);
                    JObject mData = dataObject;
                    List<myComponentProperty> currentProperty = new List<myComponentProperty>();
                    DataTable mainDataTable = new DataTable();
                    foreach (var itemData in mData)
                    {
                        DataTable mDataTable = new DataTable();
                        if (itemData.Key == "Name")
                            name = itemData.Key;
                        else if (itemData.Value.ToString().Contains("|"))
                        {
                            string dataSourceName = itemData.Value.ToString().Split('|')[0];
                            string dataSourceColoumn = itemData.Value.ToString().Split('|')[1];
                            DataSources mDataSourceInfo = allDataSourceinThisProject.Where(p => p.name.TrimEnd().ToLower() == dataSourceName.TrimEnd().ToLower()).Single();
                            mDataTable = await dtController.getCSVFileContent(mDataSourceInfo.filePath, mDataSourceInfo.columnsList, dataSourceColoumn,
                               (mDataSourceInfo.name + subProjectID));
                        }
                        if (mDataTable.Columns.Count > 0)
                            mainDataTable = mDataTable;
                        currentProperty.Add(new myComponentProperty()
                        {
                            name = itemData.Key,
                            value = itemData.Value.ToString(),
                            mDataTable = mDataTable
                        });
                    }
                    mComponentsInfoForSummation.Add(new componentsInfoForSummation()
                    {
                        componentType = className,
                        id = id,
                        inputs = inputs,
                        levelNumber = "0",
                        name = name,
                        outputs = outputs,
                        property = currentProperty,
                        mainDataTable = mainDataTable
                    });
                    //Create big DataTable with all Data
                    if (mainDataTable.Columns.Count > 0)
                    {
                        string[] columnNames_Scope_2 = mainDataTable.Columns.Cast<DataColumn>()
                                           .Select(x => x.ColumnName)
                                           .Where(x => x.ToLower().Contains("scope 2") || x.ToLower().Contains("scope_2"))
                                           .ToArray();
                        string[] columnNames_Scope_1 = mainDataTable.Columns.Cast<DataColumn>()
                                       .Select(x => x.ColumnName)
                                       .Where(x => x.ToLower().Contains("scope 1") || x.ToLower().Contains("scope_1"))
                                       .ToArray();
                        string[] columnNames_Scope_3 = mainDataTable.Columns.Cast<DataColumn>()
                                       .Select(x => x.ColumnName)
                                       .Where(x => x.ToLower().Contains("scope 3") || x.ToLower().Contains("scope_3"))
                                       .ToArray();
                        string[] columnNames_OpEx = mainDataTable.Columns.Cast<DataColumn>()
                                      .Select(x => x.ColumnName)
                                      .Where(x => x.ToLower().Contains("opex"))
                                      .ToArray();
                        string[] columnNames_CapEx = mainDataTable.Columns.Cast<DataColumn>()
                                      .Select(x => x.ColumnName)
                                      .Where(x => x.ToLower().Contains("capex") || x.ToLower().Contains("budget"))
                                      .ToArray();
                        foreach (DataRow row in mainDataTable.Rows)
                        {
                            DataRow dr = totalDataTableLevel0.NewRow();
                            dr[0] = int.Parse(row[0].ToString().Trim().Replace('-', '0'));
                            if (columnNames_Scope_2.Length > 0)
                                dr[1] = double.Parse(row[columnNames_Scope_2[0]].ToString().Trim().Replace('-', '0'));
                            else
                                dr[1] = 0;
                            if (columnNames_Scope_1.Length > 0)
                                dr[2] = double.Parse(row[columnNames_Scope_1[0]].ToString().Trim().Replace('-', '0'));
                            else
                                dr[2] = 0;
                            if (columnNames_Scope_3.Length > 0)
                                dr[3] = double.Parse(row[columnNames_Scope_3[0]].ToString().Trim().Replace('-', '0'));
                            else
                                dr[3] = 0;
                            if (columnNames_OpEx.Length > 0)
                                dr[4] = double.Parse(row[columnNames_OpEx[0]].ToString().Trim().Replace('-', '0') == "" ? "0" : row[columnNames_OpEx[0]].ToString().Trim().Replace('-', '0'));
                            else
                                dr[4] = 0;
                            if (columnNames_CapEx.Length > 0)
                                dr[5] = double.Parse(row[columnNames_CapEx[0]].ToString().Trim().Replace('-', '0'));
                            else
                                dr[5] = 0;
                            totalDataTableLevel0.Rows.Add(dr);
                        }
                    }
                }
                //Compute Summation of all Component in level3
                var newDtLevel0 = totalDataTableLevel0.AsEnumerable()
                          .GroupBy(r => r.Field<int>("Year"))
                          .Select(g =>
                          {
                              var row = totalDataTableLevel0.NewRow();
                              row["Year"] = g.Key;
                              row["Scope_2_consumption"] = g.Sum(r => r.Field<double>("Scope_2_consumption"));
                              row["Scope_1_emission"] = g.Sum(r => r.Field<double>("Scope_1_emission"));
                              row["Scope_3"] = g.Sum(r => r.Field<double>("Scope_3"));
                              row["OpEx"] = g.Sum(r => r.Field<double>("OpEx"));
                              row["CapEx"] = g.Sum(r => r.Field<double>("CapEx"));
                              return row;
                          }).CopyToDataTable();
                //
                summationListLevel0 = newDtLevel0.AsEnumerable().Select(m => new SummationModel()
                {
                    Year = m.Field<int>("Year"),
                    Scope_1_emission = m.Field<double>("Scope_1_emission"),
                    Scope_2_consumption = m.Field<double>("Scope_2_consumption"),
                    Scope_3 = m.Field<double>("Scope_3"),
                    OpEx = m.Field<double>("OpEx"),
                    CapEx = m.Field<double>("CapEx"),
                    levelNumber = "0",
                    parentNodeID = "",
                    parentNodeName = "",
                    subProjectID = subProjectID,
                    dataType = "BaseLine"
                }).ToList();
                mongoDatabase.GetCollection<SummationModel>("SummationModel").InsertMany(summationListLevel0);
                #endregion
            }
            catch (Exception M)
            {
                string t = M.Message;
            }
        }
        public class componentsInfoForSummation
        {
            public string levelNumber { get; set; }
            public string componentType { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public string inputs { get; set; }
            public string outputs { get; set; }
            public List<myComponentProperty> property { get; set; }
            public DataTable mainDataTable { get; set; }

        }
        public class myComponentProperty
        {
            public string name { get; set; }
            public string value { get; set; }
            public DataTable mDataTable { get; set; }
        }
    }
}
