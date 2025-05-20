using Motel.Models;

namespace Motel.Interfaces
{
    public interface ILoginHistoryRepository
    {
        object GetLoginHistory(int page, int pageSize);
        public void CreateLoginHistory(LoginHistory loginHistory);
        public long GetCount();
        byte[] GeneratePdfReport();
    }
}
