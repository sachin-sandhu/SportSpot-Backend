namespace SportSpot.V1.Location.Dtos
{
    public class AzureReverseResponseDto
    {
        public AzureSummaryResultDto? Summary { get; set; }
        public List<AzureReverseLocationResultDto> Addresses { get; set; } = [];
    }
}
