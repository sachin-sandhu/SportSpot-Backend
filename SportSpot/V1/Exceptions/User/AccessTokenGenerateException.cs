namespace SportSpot.V1.Exceptions.User
{
    public class AccessTokenGenerateException : AbstractSportSpotException
    {
        public AccessTokenGenerateException() : base("User.AccessTokenGenerate", "Error while generate Access Token", StatusCodes.Status500InternalServerError)
        {
        }
    }
}
