namespace SportSpot.Exceptions
{
    public abstract class AbstractSportSpotException : Exception
    {
        protected AbstractSportSpotException(string message) : base(message)
        {
        }

        protected AbstractSportSpotException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}
