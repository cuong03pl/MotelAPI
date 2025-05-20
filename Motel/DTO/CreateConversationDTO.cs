namespace Motel.DTO
{
    public class CreateConversationDTO
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
    }
}
