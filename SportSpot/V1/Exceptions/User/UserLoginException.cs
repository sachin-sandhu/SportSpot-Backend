namespace SportSpot.V1.Exceptions
{
    public class UserLoginException : AbstractSportSpotException
    {
        public UserLoginException() : base("User.Login", "Username or Password is wrong!", StatusCodes.Status401Unauthorized)
        {
        }
    }
}
