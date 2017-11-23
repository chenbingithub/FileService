using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace WebFile.Models
{
   
    public class MongoDbHelper
    {
        public MongoDatabase Database;

        public MongoDbHelper()
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mongodb"].ToString();
            var mUrl = MongoUrl.Create(connectionString);
            var serverSetting = MongoServerSettings.FromUrl(mUrl);
            Database = new MongoServer(serverSetting).GetDatabase(mUrl.DatabaseName);
        }
        public MongoDbHelper(string connectionStrings)
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStrings].ToString();
            var mUrl = MongoUrl.Create(connectionString);
            var serverSetting = MongoServerSettings.FromUrl(mUrl);
            Database = new MongoServer(serverSetting).GetDatabase(mUrl.DatabaseName);
        }
    }
}