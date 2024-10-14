namespace SportSpot.V1.Location
{
    public class AzureReverseResponseDto
    {
        public AzureSummaryResultDto? Summary { get; set; }
        public List<AzureReverseLocationResultDto> Addresses { get; set; } = [];
    }
}
