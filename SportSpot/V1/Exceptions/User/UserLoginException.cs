namespace SportSpot.V1.Exceptions.User
{
    public class UserLoginException : AbstractSportSpotException
    {
        public UserLoginException() : base("User.Login", "Username or Password is wrong!", StatusCodes.Status401Unauthorized)
        {
        }
    }
}
