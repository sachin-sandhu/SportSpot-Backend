namespace SportSpot.V1.Exceptions.User
{
    public class InvalidBirthDateException : AbstractSportSpotException
    {
        public InvalidBirthDateException() : base("User.BirthDate", $"Invalid BirthDate (min. 1900, max. {DateTime.UtcNow.Year})", StatusCodes.Status401Unauthorized)
        {
        }
    }
}
