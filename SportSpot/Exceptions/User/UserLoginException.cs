namespace SportSpot.Exceptions.User
{
    public class UserLoginException : AbstractSportSpotException
    {
        public UserLoginException() : base("Username or Password is wrong!")
        {
        }
    }
}
