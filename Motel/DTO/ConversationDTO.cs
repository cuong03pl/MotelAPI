using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Motel.Models;
using System;
using System.Collections.Generic;

namespace Motel.DTO
{
    public class ConversationDTO
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string SenderId { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public string ReceiverId { get; set; }
        
        public string LastMessageContent { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public string LastMessageSenderId { get; set; }
        
        public DateTime LastActivity { get; set; }

        public UserDTO? sender { get; set; }
        public UserDTO? receiver { get; set; }
        
        // Thêm các trường mới cho chi tiết cuộc hội thoại
        public int MessageCount { get; set; }
        public List<Message> RecentMessages { get; set; }
    }
}
