using Motel.Models;

namespace Motel.Interfaces
{
    public interface IReportRepository
    {
        public List<Reports> GetReports();
        public Reports GetReport(string id);
        public void CreateReport (Reports report);
        public bool UpdateReport (string id, Reports report);
        public bool DeleteReport (string id);
        public long CountReport ();
    }
}
