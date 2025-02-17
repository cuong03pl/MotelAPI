using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class ReportRepository : IReportRepository
    {
        public MotelService _motelService;
        public ReportRepository(MotelService motelService) {
            _motelService = motelService;
        }

        public bool Browse(string id)
        {
            var filter = Builders<Reports>.Filter.Eq(p => p.Id, id);
            var current_status = _motelService.GetReportCollection().Find(filter).Project(p => p.Status).FirstOrDefault();
            var new_sattus = (current_status == 1 ? 0 : 1);
            var update = Builders<Reports>.Update.Set("Status", new_sattus);
            var result = _motelService.GetReportCollection().UpdateOne(filter, update);
            return result.ModifiedCount > 0;
        }

        public long CountReport()
        {
           return _motelService.GetReportCollection().CountDocuments(FilterDefinition<Reports>.Empty);
        }

        public void CreateReport(Reports report)
        {
            var reports = _motelService.GetReportCollection();
            report.CreateAt = DateTime.Now;
            reports.InsertOne(report);
        }

        public bool DeleteReport(string id)
        {
           var result = _motelService.GetReportCollection().DeleteOne(report => report.Id == id);

            return result.DeletedCount > 0;
        }

        public ReportsDTO GetReport(string id)
        {
            var pipeline = new[]
             {
              new BsonDocument("$match", new BsonDocument("_id", new ObjectId(id))),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "applicationUsers" },
                    { "localField", "UserId" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Posts" },
                    { "localField", "PostId" },
                    { "foreignField", "_id" },
                    { "as", "post" }
                }),
                 new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$post" },

                }),
                new BsonDocument("$project", new BsonDocument
                    {
                        { "user", new BsonDocument
                            {
                                { "FullName", "$user.FullName" }  ,
                                { "PhoneNumber", "$user.PhoneNumber" },
                            }
                        },
                       
                    })

            };
            return _motelService.GetReportCollection().Aggregate<ReportsDTO>(pipeline).FirstOrDefault();
        }

        public async Task<object> GetReports(int page, int pageSize)
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
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Posts" },
                    { "localField", "PostId" },
                    { "foreignField", "_id" },
                    { "as", "post" }
                }),
                 new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$post" },
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
                     {"Reason", 1 },
                     {"Note", 1 },
                     {"Status", 1 },
                     {"PostId", 1 },
                     {"UserId", 1 },
                     {"post", 1 }

                    }),
                    new BsonDocument("$skip", (page-1) * pageSize),
                    new BsonDocument("$limit", pageSize),

            };
            var reports = _motelService.GetReportCollection().Aggregate<ReportsDTO>(pipeline).ToList();
            var totalCount = _motelService.GetReportCollection().CountDocuments(new BsonDocument());
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Data = reports
            };
        }

        public bool UpdateReport(string id, Reports report)
        {
            report.UpdateAt = DateTime.Now;

            var result =  _motelService.GetReportCollection().ReplaceOne(report => report.Id == id, report);
            return result.ModifiedCount > 0;
        }

       
    }
}
