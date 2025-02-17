using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Motel.Models
{
    public class Reviews
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; }

        public Guid UserId { get; set; }

        public string Comment { get; set; }
        public DateTime? CreateAt { get; set; } 
        public DateTime? UpdateAt { get; set; }
  

    }
}
