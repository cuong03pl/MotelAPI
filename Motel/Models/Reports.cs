using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    public class Reports
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        [BsonRepresentation(BsonType.ObjectId)]
        public string? PostId { get; set; }
        public Guid? UserId { get; set; }

    }
}
