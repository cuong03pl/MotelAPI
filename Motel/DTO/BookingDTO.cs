using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Motel.Models;

namespace Motel.DTO
{
    public class BookingDTO
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int Status { get; set; }
        public DateTime? CreateAt { get; set; } 
        [BsonRepresentation(BsonType.ObjectId)]
        public string? PostId { get; set; }
        public Guid? UserId { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? Price { get; set; }

        public UserDTO? user { get; set; }
        public Posts? post { get; set; }
    }
}
