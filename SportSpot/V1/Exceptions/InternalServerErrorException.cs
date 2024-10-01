namespace SportSpot.V1.Exceptions
{
    public class InternalServerErrorException : AbstractSportSpotException
    {
        public InternalServerErrorException() : base("System.InternalServerError", "Internal Server Error", StatusCodes.Status500InternalServerError)
        {
        }
    }
}