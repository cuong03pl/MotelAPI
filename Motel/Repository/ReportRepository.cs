using MongoDB.Driver;
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
        public long CountReport()
        {
           return _motelService.GetReportCollection().CountDocuments(FilterDefinition<Reports>.Empty);
        }

        public void CreateReport(Reports report)
        {
            var reports = _motelService.GetReportCollection();
            reports.InsertOne(report);
        }

        public bool DeleteReport(string id)
        {
           var result = _motelService.GetReportCollection().DeleteOne(report => report.Id == id);

            return result.DeletedCount > 0;
        }

        public Reports GetReport(string id)
        {
           return _motelService.GetReportCollection().Find(report => report.Id == id).FirstOrDefault();
        }

        public List<Reports> GetReports()
        {
            return _motelService.GetReportCollection().Find(report => true).ToList();
        }

        public bool UpdateReport(string id, Reports report)
        {
         var result =  _motelService.GetReportCollection().ReplaceOne(report => report.Id == id, report);
            return result.ModifiedCount > 0;
        }

       
    }
}
