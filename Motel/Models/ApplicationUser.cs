using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    public class ApplicationUser : MongoIdentityUser
    {
        public string? FullName { get; set; }
        public bool? IsBlock { get; set; } = false;
    }

   
}
