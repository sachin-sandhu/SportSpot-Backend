namespace SportSpot.V1.Activity.Dtos
{
    public record ActivityDto
    {
        public required Guid Id { get; init; }
        public required Guid CreatorId { get; init; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required ActivityLocationDto Location { get; set; }
        public required DateTime Date { get; set; }
        public required int MinParticipants { get; set; }
        public required int MaxParticipants { get; set; }
        public required int ParticipantsCount { get; set; }

        public required List<string> Tags { get; set; }
        public List<Guid>? Participants { get; set; }
    }
}
