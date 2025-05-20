using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    [BsonIgnoreExtraElements]
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid SenderId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ReceiverId { get; set; }

        public string Content { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        [BsonRepresentation(BsonType.ObjectId)]
        public string conversationId { get; set; }
    }
}
