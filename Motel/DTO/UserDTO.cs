using MongoDB.Bson.Serialization.Attributes;

namespace Motel.DTO
{
    public class UserDTO
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
