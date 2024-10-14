namespace SportSpot.V1.Exceptions
{
    public class UserNotFoundException : AbstractSportSpotException
    {
        public UserNotFoundException() : base("User.UserNotFound", "User not found", StatusCodes.Status404NotFound)
        {
        }
    }
}
