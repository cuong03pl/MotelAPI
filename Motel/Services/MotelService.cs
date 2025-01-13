﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Models;

namespace Motel.Services
{
    public class MotelService
    {
        public IMongoDatabase _database { get; set; }
        public MotelService(IOptions<MotelDatabaseSettings> options) { 
            var databaseSettings = new MongoClient(options.Value.ConnectionString);
            _database = databaseSettings.GetDatabase(options.Value.DatabaseName);
        }

        public IMongoCollection<Posts> GetPostCollection()
        {
            return _database.GetCollection<Posts>("Posts");
        }
        public IMongoCollection<ApplicationUser> GetUserCollection()
        {
            return _database.GetCollection<ApplicationUser>("applicationUsers");
        }
        public IMongoCollection<Reports> GetReportCollection()
        {
            return _database.GetCollection<Reports>("Reports");
        }

    }
}
