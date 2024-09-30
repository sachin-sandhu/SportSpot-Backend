namespace SportSpot.Exceptions.User
{
    public class AccessTokenGenerateException : AbstractSportSpotException
    {
        public AccessTokenGenerateException() : base("Error while generate Access Token")
        {
        }
    }
}
