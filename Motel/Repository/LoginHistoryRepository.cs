using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Helpers;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class LoginHistoryRepository : ILoginHistoryRepository
    {
        public MotelService _motelService;
        public LoginHistoryRepository(MotelService motelService) {
            _motelService = motelService;
        }
       
        public void CreateLoginHistory(LoginHistory loginHistory)
        {
            var loginHistories = _motelService.GetLoginCollection();
            loginHistories.InsertOne(loginHistory);
        }

        public byte[] GeneratePdfReport()
        {
            throw new NotImplementedException();
        }

        public long GetCount()
        {
            throw new NotImplementedException();
        }

        public object GetLoginHistory(int page, int pageSize)
        {
            var pipeline = new[]
             {
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "UserId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                 new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),
                new BsonDocument("$project", new BsonDocument
                    {
                        { "user", new BsonDocument
                            {
                                { "FullName", "$user.FullName" }  ,
                                { "PhoneNumber", "$user.PhoneNumber" },
                            }
                        },
                        {"IpAddress", 1 },
                        {"UserAgent", 1 },
                        {"LoginTime", 1 }
                    }),
                 new BsonDocument("$skip", (page - 1) * pageSize),
                new BsonDocument("$limit", pageSize),
            };
            var loginHistories = _motelService.GetLoginCollection().Aggregate<LoginHistoryDTO>(pipeline).ToList();
            var totalCount = _motelService.GetLoginCollection().CountDocuments(new BsonDocument());
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return new
            {
                Data = loginHistories,
                TotalCount = totalCount,
                TotalPages = totalPages,
            };
        }
    }
}
