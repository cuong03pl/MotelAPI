using MongoDB.Driver;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;

namespace Motel.Repository
{
    public class UserRepository : IUserRepository
    {
        public MotelService _motelService { get; set; }
        public UserRepository(MotelService motelService) { 
            _motelService = motelService;
        }

        public void DeleteUser(Guid id)
        {
            _motelService.GetUserCollection().DeleteOne(user => user.Id == id);

        }

        public ApplicationUser GetUser(Guid id)
        {
           return _motelService.GetUserCollection().Find(user => user.Id == id).FirstOrDefault();
        }

        public List<ApplicationUser> GetUsers()
        {
            return _motelService.GetUserCollection().Find(user => true).ToList();
        }

        public void UpdateUser(Guid id, ApplicationUser user)
        {
            _motelService.GetUserCollection().ReplaceOne(user => user.Id == id, user);
        }

        public void BlockUser(Guid id, bool is_block)
        {
            var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, id);
            var update = Builders<ApplicationUser>.Update.Set("IsBlock", is_block);
            var result = _motelService.GetUserCollection().UpdateOne(filter, update);
        }
    }
}
