namespace SportSpot.V1.Exceptions.User
{
    public class ClubNotFoundException : AbstractSportSpotException
    {
        public ClubNotFoundException() : base("User.ClubNotFound", "Club not found", StatusCodes.Status404NotFound)
        {
        }
    }
}
