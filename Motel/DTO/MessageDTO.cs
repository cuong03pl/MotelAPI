namespace Motel.DTO
{
    public class MessageDTO
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; }
        public string ConversationId { get; set; }

    }
}
