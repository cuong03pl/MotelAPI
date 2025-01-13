using Motel.Models;

namespace Motel.Interfaces
{
    public interface IUserRepository
    {
        public List<ApplicationUser> GetUsers();
        public ApplicationUser GetUser(Guid id);
        public void UpdateUser(Guid id, ApplicationUser user);
        public void DeleteUser(Guid id);
        public void BlockUser(Guid id, bool is_block);
    }
}
