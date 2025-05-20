using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Motel.Models
{
    public class Conversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public Guid SenderId { get; set; }
        
        public Guid ReceiverId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime LastActivity { get; set; }

        public string LastMessageContent { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid LastMessageSenderId { get; set; }
    }
}
