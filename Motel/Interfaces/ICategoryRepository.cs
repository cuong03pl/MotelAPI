using Motel.DTO;
using Motel.Models;

namespace Motel.Interfaces
{
    public interface ICategoryRepository
    {
        List<Categories> GetCategories();
        public void CreateCategory(Categories category);
        public void UpdateCategory(string id, Categories category);
        public void DeleteCategory(string id);
        public Categories GetCategory(string slug);
        public long GetCount();
    }
}
