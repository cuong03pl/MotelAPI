using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    public class ApplicationUser : MongoIdentityUser
    {
        public string? FullName { get; set; }
        public bool? IsBlock { get; set; } = false;
        public string? Avatar { get; set; }
        public bool IsVerified { get; set; } = false;

        public List<string> Favorites { get; set; } = new List<string>();
    }

   
}
