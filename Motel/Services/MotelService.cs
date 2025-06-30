using Microsoft.Extensions.Options;
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
        public IMongoCollection<ApplicationRole> GetRoleCollection()
        {
            return _database.GetCollection<ApplicationRole>("applicationRoles");
        }
        public IMongoCollection<Reports> GetReportCollection()
        {
            return _database.GetCollection<Reports>("Reports");
        }
        public IMongoCollection<Reviews> GetReviewCollection()
        {
            return _database.GetCollection<Reviews>("Reviews");
        }

        public IMongoCollection<Categories> GetCategoryCollection()
        {
            return _database.GetCollection<Categories>("Categories");
        }
        public IMongoCollection<News> GetNewsCollection()
        {
            return _database.GetCollection<News>("News");
        }
        public IMongoCollection<Bookings> GetBookingCollection()
        {
            return _database.GetCollection<Bookings>("Bookings");
        }
        public IMongoCollection<LoginHistory> GetLoginCollection()
        {
            return _database.GetCollection<LoginHistory>("LoginHistory");
        }

        public IMongoCollection<Conversation> GetConversationCollection()
        {
            return _database.GetCollection<Conversation>("Conversation");
        }
        public IMongoCollection<Message> GetMessageCollection()
        {
            return _database.GetCollection<Message>("Message");
        }
    }
}
