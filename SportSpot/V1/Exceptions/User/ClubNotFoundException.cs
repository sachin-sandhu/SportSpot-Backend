namespace SportSpot.V1.Exceptions
{
    public class ClubNotFoundException : AbstractSportSpotException
    {
        public ClubNotFoundException() : base("User.ClubNotFound", "Club not found", StatusCodes.Status404NotFound)
        {
        }
    }
}
