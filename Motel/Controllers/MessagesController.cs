using Microsoft.AspNetCore.Mvc;
using Motel.DTO;
using Motel.Interfaces;
using Motel.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Motel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessagesController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpGet("messages/{conversationId}")]
        public async Task<IActionResult> GetMessages(string conversationId)
        {
            var messages = await _messageRepository.GetMessagesByConversationId(conversationId);
            return Ok(messages);
        }

        [HttpGet("conversations/{userId}")]
        public async Task<IActionResult> GetConversations(Guid userId)
        {
            try
            {
                var conversations = await _messageRepository.GetConversationsForUser(userId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("conversation/{conversationId}")]
        public async Task<IActionResult> GetConversationDetail(string conversationId)
        {
            try
            {
                var conversation = await _messageRepository.GetConversationDetail(conversationId);
                if (conversation == null)
                    return NotFound(new { message = "Không tìm thấy cuộc hội thoại này" });
                
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/chat/send
        // [HttpPost("send")]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageDTO model)
        {
            try
            {
                var message = new Message
                {
                    SenderId = model.SenderId,
                    ReceiverId = model.ReceiverId,
                    Content = model.Content,
                    conversationId = model.ConversationId,
                    Timestamp = DateTime.UtcNow,
                    IsRead = false
                };

                await _messageRepository.SaveMessage(message);
                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // [HttpPost("create")]
        [HttpPost("conversations")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDTO model)
        {
            try
            {
                var conversation = await _messageRepository.CreateConversation(model.SenderId, model.ReceiverId);
                return Ok(conversation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
} 