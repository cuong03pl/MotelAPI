using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Motel.DTO
{
    public class UserDTO
    {
        [BsonId]
        [BsonElement("_id")]
        public string Id { get; set; }
        
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
