namespace SportSpot.V1.Session.Chat.Dtos
{
    public record MessageDto
    {
        public required Guid Id { get; set; }
        public required Guid SessionId { get; set; }
        public required Guid CreatorId { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? ParentMessageId { get; set; }
    }
}
