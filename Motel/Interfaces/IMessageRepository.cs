using Motel.DTO;
using Motel.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Motel.Interfaces
{
    public interface IMessageRepository
    {
        Task SaveMessage(Message message);
        Task<List<Message>> GetMessagesByConversationId(string conversationId);
        Task<Conversation> CreateConversation(Guid senderId, Guid receiverId);
        Task<bool> UpdateConversationLastActivity(string conversationId, string lastMessage, Guid senderId);
        Task<List<ConversationDTO>> GetConversationsForUser(Guid userId);
        Task<ConversationDTO> GetConversationDetail(string conversationId);
        Task<bool> MarkMessagesAsRead(string conversationId, Guid userId);
    }
} 