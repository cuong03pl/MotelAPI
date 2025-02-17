using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Motel.Models;

namespace Motel.DTO
{
    public class ReportsDTO
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public int Status { get; set; } = 0;
        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        [BsonRepresentation(BsonType.ObjectId)]
        public string? PostId { get; set; }
        public Guid? UserId { get; set; }

        public UserDTO? user { get; set; }
        public Posts? post { get; set; }
    }
}
