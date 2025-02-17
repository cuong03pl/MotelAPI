using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    public class News
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Slug { get; set; }
        public DateTime? CreateAt { get; set; } 
        public DateTime? UpdateAt { get; set; }
    }
}
