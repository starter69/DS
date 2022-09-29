using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using DSider.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DSider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAPI_UploaderController : ControllerBase
    {
        private readonly IOptions<DatabaseSettings> appSettings;
        private IMongoDatabase mongoDatabase;
        //Initialize MongoDB Connections From appsettings.json
        public IMongoDatabase GetMongoDatabase()
        {
            string ConnectionString = appSettings.Value.ConnectionString;
            string DbName = appSettings.Value.DatabaseName;
            var mongoClient = new MongoClient(ConnectionString);
            return mongoClient.GetDatabase(DbName);
        }
        private IHostingEnvironment hostingEnv;
        public WebAPI_UploaderController(IHostingEnvironment env, IOptions<DatabaseSettings> app)
        {
            this.hostingEnv = env;
            appSettings = app;
        }
        //Upload datasource csv file to S3
        [Route("Upload_File")]
        [HttpPost]
        public async Task<string> Upload_FileAsync()
        {
            string result = string.Empty;
            try
            {
                string DataSourceName = Request.Cookies["DataSourceName"];
                DataSourceName = DataSourceName.Replace("#@", " ");
                //
                string DataSourceDescription = Request.Cookies["DataSourceDescription"];
                DataSourceDescription = DataSourceDescription.Replace("#@", " ");
                //
                string subProjectID = Request.Cookies["subProjectID"];
                mongoDatabase = GetMongoDatabase();
                //
                var filterDataSources = Builders<DataSources>.Filter.Eq("subProjectID", subProjectID);
                List<DataSources> allDataSourceinThisProject = mongoDatabase.GetCollection<DataSources>("DataSources").Find(filterDataSources).ToList();
                //
                allDataSourceinThisProject = allDataSourceinThisProject.Where(p => p.name.ToLower() == DataSourceName.ToLower()).ToList();
                if (allDataSourceinThisProject.Count > 0)
                    return "Error Name";
                long size = 0;
                var file = Request.Form.Files;
                var filename = ContentDispositionHeaderValue
                                .Parse(file[0].ContentDisposition)
                                .FileName
                                .Trim('"');
                string FilePath = hostingEnv.WebRootPath + $@"\{ filename}";
                size += file[0].Length;
                //using (FileStream fs = System.IO.File.Create(FilePath))
                //{
                //    file[0].CopyTo(fs);
                //    fs.Flush();
                //}
                foreach (IFormFile mFile in Request.Form.Files)
                {
                    await UploadFileToS3Async(mFile);
                }
                result = FilePath;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        //Upload user defined object icons to S3
        [Route("Upload_UserDefinedObject")]
        [HttpPost]
        public async Task<string> Upload_UserDefinedObject()
        {
            string result = string.Empty;
            try
            {
                string ObjecID = Request.Cookies["ObjecID"];
                mongoDatabase = GetMongoDatabase();
                //
                foreach (IFormFile mFile in Request.Form.Files)
                {
                    await UploadObjectIconToS3Async(mFile);
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        //Upload JSON file to S3 when import json file
        [Route("UploadJSONFile")]
        [HttpPost]
        public async Task<string> UploadJSONFile()
        {
            string result = string.Empty;
            try
            {
                string subProjectID = Request.Cookies["subProject"];
                mongoDatabase = GetMongoDatabase();
                //
                //
                long size = 0;
                var file = Request.Form.Files;
                var filename = ContentDispositionHeaderValue
                                .Parse(file[0].ContentDisposition)
                                .FileName
                                .Trim('"');
                string FilePath = hostingEnv.WebRootPath + $@"\{ filename}";
                size += file[0].Length;
                //
                foreach (IFormFile mFile in Request.Form.Files)
                {
                    MemoryStream newMemoryStream = new MemoryStream();
                    mFile.CopyTo(newMemoryStream);
                    string allText = Encoding.UTF8.GetString(newMemoryStream.GetBuffer(), 0, (int)newMemoryStream.Length);
                    //
                    var filter = Builders<SubProjects>.Filter.Eq("id", subProjectID);
                    var updatestatement = Builders<SubProjects>.Update.Set("exportJSON", allText);
                    mongoDatabase.GetCollection<SubProjects>("SubProjects").UpdateMany(filter, updatestatement);
                    result = allText;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static IAmazonS3 s3Client;
        public async Task<bool> UploadFileToS3Async(IFormFile file)
        {
            mongoDatabase = GetMongoDatabase();
            bool result = true;
            //
            try
            {
                string DataSourceName = Request.Cookies["DataSourceName"];
                DataSourceName = DataSourceName.Replace("#@", " ");
                //
                string DataSourceDescription = Request.Cookies["DataSourceDescription"];
                DataSourceDescription = DataSourceDescription.Replace("#@", " ");
                //
                string subProjectID = Request.Cookies["subProjectID"];
                string userName = Request.Cookies["userName"];
                string path = userName + "/" + DataSourceName + "/";
                // get from environment
                using (var client = new AmazonS3Client("AKIA2NWMPB72LIDOX4RP", "/gBcBBxIFA8ExrhHPIlo1SGSu0jwxoj3xTmFT1un", RegionEndpoint.USEast1))
                {
                    MemoryStream newMemoryStream = new MemoryStream();
                    file.CopyTo(newMemoryStream);
                    //Get Columns
                    string allText = Encoding.UTF8.GetString(newMemoryStream.GetBuffer(), 0, (int)newMemoryStream.Length);
                    string firstLine = allText.Split("\n")[0];
                    string[] columnsList = firstLine.Split(',');
                    columnsList = columnsList.Where(p => !p.ToLower().Contains("time") && !p.ToLower().Contains("date")).ToArray();
                    //End Column
                    string mFileName = DataSourceName + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".csv";
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = path + mFileName,
                        BucketName = "dsiderinc-bucket",
                        CannedACL = S3CannedACL.PublicReadWrite
                    };
                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                    //
                    DataSources dataSourceModel = new DataSources();
                    //
                    //Insert MongoDB
                    //
                    ObjectId mID = ObjectId.GenerateNewId();
                    dataSourceModel.createDate = DateTime.Now.ToString();
                    dataSourceModel.userCreator = userName;
                    dataSourceModel._id = mID;
                    dataSourceModel.id = mID.ToString();
                    dataSourceModel.description = DataSourceDescription;
                    dataSourceModel.filePath = path + mFileName;
                    dataSourceModel.name = DataSourceName;
                    dataSourceModel.subProjectID = subProjectID;
                    dataSourceModel.columnsList = columnsList;
                    mongoDatabase.GetCollection<DataSources>("DataSources").InsertOne(dataSourceModel);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public async Task<bool> UploadObjectIconToS3Async(IFormFile file)
        {
            mongoDatabase = GetMongoDatabase();
            bool result = true;
            //
            try
            {
                string fileExtension = Path.GetExtension(file.FileName);
                string ObjecID = Request.Cookies["ObjecID"];
                //
                string userName = Request.Cookies["userName"];
                using (var client = new AmazonS3Client("AKIA2NWMPB72LIDOX4RP", "/gBcBBxIFA8ExrhHPIlo1SGSu0jwxoj3xTmFT1un", RegionEndpoint.USEast1))
                {
                    MemoryStream newMemoryStream = new MemoryStream();
                    file.CopyTo(newMemoryStream);
                    string mFileName = "objects-icons/" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + fileExtension;
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key =  mFileName,
                        BucketName = "dsiderinc-bucket",
                        CannedACL = S3CannedACL.PublicReadWrite
                    };
                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                    //
                    FreeStyleObjects dataSourceModel = new FreeStyleObjects();
                    //
                    //Insert MongoDB
                    //
                    var filter = Builders<FreeStyleObjects>.Filter.Eq("id", ObjecID);
                    var updatestatement = Builders<FreeStyleObjects>.Update.Set("imagePath", mFileName);
                    var resultSave = mongoDatabase.GetCollection<FreeStyleObjects>("FreeStyleObjects").UpdateMany(filter, updatestatement);
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
