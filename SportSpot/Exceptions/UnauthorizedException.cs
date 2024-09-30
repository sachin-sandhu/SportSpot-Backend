namespace SportSpot.Exceptions
{
    public class UnauthorizedException : AbstractSportSpotException
    {
        public UnauthorizedException() : base("Unauthorized")
        {
        }
    }
}
