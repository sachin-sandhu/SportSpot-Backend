namespace SportSpot.V1.Location.Dtos
{
    public class AzureSearchResponseDto
    {
        public AzureSummaryResultDto? Summary { get; set; }
        public List<AzureLocationResultDto> Results { get; set; } = [];
    }
}
