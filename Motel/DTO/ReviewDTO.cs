using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Motel.Models;

namespace Motel.DTO
{
    public class ReviewDTO
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? PostId { get; set; }

        public Guid? UserId { get; set; }

        public string? Comment { get; set; }
        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public UserDTO? user { get; set; }
        public Posts? post { get; set; }
    }
}
