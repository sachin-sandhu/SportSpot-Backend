namespace SportSpot.V1.Exceptions
{
    public class UnauthorizedException : AbstractSportSpotException
    {
        public UnauthorizedException() : base("System.Unauthorized", "Unauthorized", StatusCodes.Status401Unauthorized)
        {
        }
    }
}
