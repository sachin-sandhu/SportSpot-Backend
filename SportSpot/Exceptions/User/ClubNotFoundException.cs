namespace SportSpot.Exceptions.User
{
    public class ClubNotFoundException : AbstractSportSpotException
    {
        public ClubNotFoundException() : base("Club not found")
        {
        }
    }
}
