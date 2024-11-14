namespace SportSpot.V1.Location.Dtos
{
    public class AzureSummaryResultDto
    {
        public required string Query { get; set; }
        public required string QueryType { get; set; }
        public int QueryTime { get; set; }
        public int NumResults { get; set; }
        public int Offset { get; set; }
        public int TotalResults { get; set; }
        public int FuzzyLevel { get; set; }
    }
}
