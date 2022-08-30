using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Caching.Memory;
using Sylvan.Data.Csv;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Controllers
{
    public class DataSourcesController
    {
        private readonly IMemoryCache memoryCache;
        public DataSourcesController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        //Get DataSources from csv to DataTable from s3 by S3 Address
        public async Task<DataTable> getCSVFileContent(string filePath, string[] coloumnsList, string dataSourceColoumn, string dataSourceNameSubProjectCacheKey)
        {
            DataTable csvDataTable = new DataTable();
            Cache.CacheController CacheCon = new Cache.CacheController(memoryCache);
            try
            {
                //Check Cache
                //if Exists in Cache return from Cache Else Get csv from S3
                if (memoryCache.TryGetValue(dataSourceNameSubProjectCacheKey, out DataTable csvFileContent))
                {
                    return CacheCon.GetCache(dataSourceNameSubProjectCacheKey);
                }
                //
                var awsAccessKey = "AKIA2NWMPB72LIDOX4RP";
                var awsSecretKey = "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";
                var region = Amazon.RegionEndpoint.USEast1;
                using (var client = new AmazonS3Client(awsAccessKey, awsSecretKey, region))
                {
                    GetObjectRequest request = new GetObjectRequest();
                    request.BucketName = "dsiderinc-bucket";
                    request.Key = filePath;
                    GetObjectResponse response = await client.GetObjectAsync(request);

                    StreamReader sr = new StreamReader(response.ResponseStream);
                    //
                    int counter = 0;
                    var schema = new TypedCsvSchema();
                    //
                    //csvDataTable.Columns.Add("DateTime");
                    //csvDataTable.Columns.Add(dataSourceColoumn);
                    schema.Add(typeof(string));
                    schema.Add(typeof(string));
                    //
                    //foreach (string header in coloumnsList)
                    //{
                    //    if (header.Trim() != "")
                    //    {
                    //        csvDataTable.Columns.Add(header);
                    //        schema.Add(typeof(string));
                    //        //
                    //        counter++;
                    //    }
                    //}
                    using var csv = CsvDataReader.Create(sr, new CsvDataReaderOptions { Schema = schema });
                    csvDataTable.Load(csv);
                    csvDataTable.Columns[0].ColumnName = "DateTime";
                    //csvDataTable.Columns[1].ColumnName = dataSourceColoumn;
                    //counter = 0;
                    //foreach (string header in coloumnsList)
                    //{
                    //    if (header.Trim() != "")
                    //    {
                    //        csvDataTable.Columns[counter].ColumnName = header;
                    //        counter++;
                    //    }
                    //}
                    //csvDataTable.Rows[0].Delete();
                    csvDataTable.AcceptChanges();
                    //
                }
            }
            catch (Exception M)
            {
                string t = M.Message;
                throw;
            }
            //Set Cache
            CacheCon.SetCache(new Cache.CacheController.CacheRequest()
            {
                key = dataSourceNameSubProjectCacheKey,
                value = csvDataTable
            });
            return csvDataTable;
        }
    }
    class TypedCsvColumn : DbColumn
    {
        public TypedCsvColumn(Type type, bool allowNull)
        {
            // if you assign ColumnName here, it will override whatever is in the csv header
            this.DataType = type;
            this.AllowDBNull = allowNull;
        }

        public Type DataType { get; }
    }

    class TypedCsvSchema : ICsvSchemaProvider
    {
        List<TypedCsvColumn> columns;

        public TypedCsvSchema()
        {
            this.columns = new List<TypedCsvColumn>();
        }

        public TypedCsvSchema Add(Type type, bool allowNull = false)
        {
            this.columns.Add(new TypedCsvColumn(type, allowNull));
            return this;
        }

        DbColumn? ICsvSchemaProvider.GetColumn(string? name, int ordinal)
        {
            return ordinal < columns.Count ? columns[ordinal] : null;
        }
    }
}
