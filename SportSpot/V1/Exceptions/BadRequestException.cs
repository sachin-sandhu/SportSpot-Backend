namespace SportSpot.V1.Exceptions
{
    public class BadRequestException(string message) : AbstractSportSpotException("System.BadRequest", message, StatusCodes.Status400BadRequest)
    {
    }
}
