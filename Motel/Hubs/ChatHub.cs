using Microsoft.AspNetCore.SignalR;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;
using System.Threading.Tasks;
using System;
using MongoDB.Bson;

namespace Motel.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MotelService _motelService;
        private readonly IMessageRepository _messageRepository;

        public ChatHub(MotelService motelService, IMessageRepository messageRepository)
        {
            _motelService = motelService;
            _messageRepository = messageRepository;
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task SendMessageToConversation(string senderId, string receiverId, string conversationId, string message)
        {
            var chat = new Message
            {
                SenderId = Guid.Parse(senderId),
                ReceiverId = Guid.Parse(receiverId),
                Content = message,
                conversationId = conversationId,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            await _messageRepository.SaveMessage(chat);

            // Gửi các trường riêng biệt thay vì gửi cả đối tượng chat
            await Clients.Group(conversationId).SendAsync(
                "ReceiveMessage",
                senderId,
                message,
                conversationId,
                chat.Timestamp.ToString("o"),
                chat.Id.ToString()
            );
        }

        public async Task MarkMessagesAsRead(string conversationId, string userId)
        {
            if (string.IsNullOrEmpty(conversationId) || string.IsNullOrEmpty(userId))
            {
                return;
            }

            var success = await _messageRepository.MarkMessagesAsRead(conversationId, Guid.Parse(userId));
            
            if (success)
            {
                // Notification is already sent in the repository method
                // This is just for logging purposes
                Console.WriteLine($"Messages marked as read in conversation {conversationId} for user {userId}");
            }
        }
        
        public async Task SendMessageToGroup(string groupId, string senderId, string message)
        {
            if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(message))
            {
                return;
            }
            
            var timestamp = DateTime.UtcNow;
            var messageId = ObjectId.GenerateNewId().ToString();
            
            // Send message to all clients in the group
            await Clients.Group(groupId).SendAsync(
                "ReceiveMessage",
                senderId,
                message,
                groupId,
                timestamp.ToString("o"),
                messageId
            );
            
            // This is a backup method - you should consider using SendMessageToConversation instead
            // as it properly saves the message to the database
            Console.WriteLine($"Message sent to group {groupId} from {senderId}");
        }
    }
}
