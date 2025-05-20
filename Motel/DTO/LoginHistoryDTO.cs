using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.DTO
{
    public class LoginHistoryDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime LoginTime { get; set; }
        public UserDTO? user { get; set; }
    }
}
