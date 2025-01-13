using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    public class Bookings
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }  

        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; } 

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }  

        public string Status { get; set; }  

        public DateTime CreateAt { get; set; } 
    }
}
