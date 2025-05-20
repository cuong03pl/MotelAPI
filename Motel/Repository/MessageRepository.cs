using MongoDB.Bson;
using MongoDB.Driver;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using Motel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Motel.Hubs;
using AutoMapper;

namespace Motel.Repository
{
    
    public class MessageRepository : IMessageRepository
    {
        private readonly MotelService _motelService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMapper _mapper;

        public MessageRepository(MotelService motelService, IHubContext<ChatHub> hubContext, IMapper mapper)
        {
            _motelService = motelService;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task<Conversation> CreateConversation(Guid senderId, Guid receiverId)
        {
            // Kiểm tra xem cuộc trò chuyện đã tồn tại chưa
            var existingConversation = await _motelService.GetConversationCollection()
                .Find(c => (c.SenderId == senderId && c.ReceiverId == receiverId) || 
                           (c.SenderId == receiverId && c.ReceiverId == senderId))
                .FirstOrDefaultAsync();

            if (existingConversation != null)
            {
                return existingConversation;
            }

            var conversation = new Conversation
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                LastActivity = DateTime.UtcNow,
                LastMessageContent = "",
                LastMessageSenderId = senderId
            };

            await _motelService.GetConversationCollection().InsertOneAsync(conversation);
            return conversation;
        }

        public async Task<List<Message>> GetMessagesByConversationId(string conversationId)
        {
            return await _motelService.GetMessageCollection()
            .Find(m => m.conversationId == conversationId)
            .SortBy(m => m.Timestamp)
            .ToListAsync();
        }

        public async Task SaveMessage(Message message)
        {
            // Lưu tin nhắn
            await _motelService.GetMessageCollection().InsertOneAsync(message);
            
            // Cập nhật thông tin cuộc trò chuyện
            await UpdateConversationLastActivity(message.conversationId, message.Content, message.SenderId);
        }

        public async Task<bool> UpdateConversationLastActivity(string conversationId, string lastMessage, Guid senderId)
        {
            try
            {
                // Đảm bảo conversationId là valid
                if (!ObjectId.TryParse(conversationId, out _))
                {
                    // Log lỗi nếu cần
                    return false;
                }

                var filter = Builders<Conversation>.Filter.Eq("_id", ObjectId.Parse(conversationId));
                
                // Tạo thông tin cập nhật
                var updateDefinition = Builders<Conversation>.Update
                    .Set(c => c.LastActivity, DateTime.UtcNow)
                    .Set(c => c.LastMessageContent, lastMessage)
                    .Set(c => c.LastMessageSenderId, senderId);

                // Thực hiện cập nhật và lấy kết quả
                var options = new UpdateOptions { IsUpsert = false };
                var result = await _motelService.GetConversationCollection().UpdateOneAsync(filter, updateDefinition, options);
                
                // Debug: Kiểm tra xem cập nhật có thành công không
                if (result.MatchedCount == 0)
                {
                    // Không tìm thấy đối tượng để cập nhật
                    return false;
                }
                
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return false;
            }
        }

        public async Task<List<ConversationDTO>> GetConversationsForUser(Guid userId)
        {
            var filter = Builders<Conversation>.Filter.Or(
                Builders<Conversation>.Filter.Eq(c => c.SenderId, userId),
                Builders<Conversation>.Filter.Eq(c => c.ReceiverId, userId)
            );

            var conversations = await _motelService.GetConversationCollection()
                .Find(filter)
                .SortByDescending(c => c.LastActivity)
                .ToListAsync();

            var conversationDTOs = new List<ConversationDTO>();

            foreach (var conversation in conversations)
            {
                var conversationDTO = new ConversationDTO
                {
                    Id = conversation.Id,
                    SenderId = conversation.SenderId.ToString(),
                    ReceiverId = conversation.ReceiverId.ToString(),
                    LastMessageContent = conversation.LastMessageContent,
                    LastMessageSenderId = conversation.LastMessageSenderId.ToString(),
                    LastActivity = conversation.LastActivity
                };

                // Lấy thông tin người gửi
                var sender = await _motelService.GetUserCollection()
                    .Find(u => u.Id.ToString() == conversation.SenderId.ToString())
                    .FirstOrDefaultAsync();
                if (sender != null)
                {
                    conversationDTO.sender = new UserDTO
                    {
                        Id = sender.Id.ToString(),
                        FullName = sender.FullName,
                        PhoneNumber = sender.PhoneNumber,
                        Avatar = sender.Avatar,
                        CreatedOn = sender.CreatedOn
                    };
                }

                // Lấy thông tin người nhận
                var receiver = await _motelService.GetUserCollection()
                    .Find(u => u.Id.ToString() == conversation.ReceiverId.ToString())
                    .FirstOrDefaultAsync();
                if (receiver != null)
                {
                    conversationDTO.receiver = new UserDTO
                    {
                        Id = receiver.Id.ToString(),
                        FullName = receiver.FullName,
                        PhoneNumber = receiver.PhoneNumber,
                        Avatar = receiver.Avatar,
                        CreatedOn = receiver.CreatedOn
                    };
                }

                conversationDTOs.Add(conversationDTO);
            }

            return conversationDTOs;
        }
        
        public async Task<ConversationDTO> GetConversationDetail(string conversationId)
        {
            try
            {
                // Truy vấn để lấy cuộc hội thoại theo ID
                var conversation = await _motelService.GetConversationCollection()
                    .Find(c => c.Id == conversationId)
                    .FirstOrDefaultAsync();

                if (conversation == null)
                    return null;

                // Tạo DTO để trả về
                var conversationDTO = new ConversationDTO
                {
                    Id = conversation.Id,
                    SenderId = conversation.SenderId.ToString(),
                    ReceiverId = conversation.ReceiverId.ToString(),
                    LastMessageContent = conversation.LastMessageContent,
                    LastMessageSenderId = conversation.LastMessageSenderId.ToString(),
                    LastActivity = conversation.LastActivity
                };

                // Lấy thông tin người gửi
                var sender = await _motelService.GetUserCollection()
                    .Find(u => u.Id.ToString() == conversation.SenderId.ToString())
                    .FirstOrDefaultAsync();
                if (sender != null)
                {
                    conversationDTO.sender = new UserDTO
                    {
                        Id = sender.Id.ToString(),
                        FullName = sender.FullName,
                        PhoneNumber = sender.PhoneNumber,
                        Avatar = sender.Avatar,
                        CreatedOn = sender.CreatedOn
                    };
                }

                // Lấy thông tin người nhận
                var receiver = await _motelService.GetUserCollection()
                    .Find(u => u.Id.ToString() == conversation.ReceiverId.ToString())
                    .FirstOrDefaultAsync();
                if (receiver != null)
                {
                    conversationDTO.receiver = new UserDTO
                    {
                        Id = receiver.Id.ToString(),
                        FullName = receiver.FullName,
                        PhoneNumber = receiver.PhoneNumber,
                        Avatar = receiver.Avatar,
                        CreatedOn = receiver.CreatedOn
                    };
                }

                // Lấy số lượng tin nhắn
                var messageCount = await _motelService.GetMessageCollection()
                    .CountDocumentsAsync(m => m.conversationId == conversation.Id);
                
                // Lấy thông tin của 10 tin nhắn mới nhất
                var recentMessages = await _motelService.GetMessageCollection()
                    .Find(m => m.conversationId == conversation.Id)
                    .SortByDescending(m => m.Timestamp)
                    .Limit(10)
                    .ToListAsync();

                // Thêm thông tin bổ sung vào DTO
                conversationDTO.MessageCount = (int)messageCount;
                conversationDTO.RecentMessages = recentMessages;

                return conversationDTO;
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                return null;
            }
        }

        public async Task<bool> MarkMessagesAsRead(string conversationId, Guid userId)
        {
            try
            {
                var filter = Builders<Message>.Filter.And(
                    Builders<Message>.Filter.Eq(m => m.conversationId, conversationId),
                    Builders<Message>.Filter.Eq(m => m.ReceiverId, userId),
                    Builders<Message>.Filter.Eq(m => m.IsRead, false)
                );

                var update = Builders<Message>.Update.Set(m => m.IsRead, true);
                
                var result = await _motelService.GetMessageCollection().UpdateManyAsync(filter, update);
                
                // Notify other clients about read status update
                if (result.ModifiedCount > 0)
                {
                    await _hubContext.Clients.Group(conversationId)
                        .SendAsync("MessagesRead", conversationId, userId.ToString());
                }
                
                return result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking messages as read: {ex.Message}");
                return false;
            }
        }
    }
} 