
namespace SportSpot.V1.Exceptions.Session
{
    public class SessionInvalidPageException : AbstractSportSpotException
    {
        public SessionInvalidPageException() : base("Session.InvalidPage", "Page or Size is not valid!", StatusCodes.Status400BadRequest)
        {
        }
    }
}
