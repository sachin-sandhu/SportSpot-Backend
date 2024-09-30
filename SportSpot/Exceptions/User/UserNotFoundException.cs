namespace SportSpot.Exceptions.User
{
    public class UserNotFoundException : AbstractSportSpotException
    {
        public UserNotFoundException() : base("User not found")
        {
        }
    }
}
