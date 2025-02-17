using Motel.Models;

namespace Motel.Interfaces
{
    public interface INewsRepository
    {
        List<News> GetNews(int page, int pageSize);
        News GetNews(string slug);
        public void CreateNews(News news);
        public void UpdateNews(string id, News news);
        public void DeleteNews(string id);
        public long GetCount();

    }
}
