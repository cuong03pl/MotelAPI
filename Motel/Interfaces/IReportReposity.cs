using Motel.DTO;
using Motel.Models;

namespace Motel.Interfaces
{
    public interface IReportRepository
    {
         Task<object> GetReports(int page, int pageSize);
        public ReportsDTO GetReport(string id);
        public void CreateReport (Reports report);
        public bool UpdateReport (string id, Reports report);
        public bool DeleteReport (string id);
        public long CountReport ();
        byte[] GeneratePdfReport();
        public bool Browse(string id);
        Task<List<ReportReasonCount>> GetReportsByReason();
    }
}
