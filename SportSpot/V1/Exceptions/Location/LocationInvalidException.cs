namespace SportSpot.V1.Exceptions.Location
{
    public class LocationInvalidException : AbstractSportSpotException
    {
        public LocationInvalidException() : base("Location.Invalid", "Invalid Location. Latitude must be between -90 and 90, Longitude must be between -180 and 180.", StatusCodes.Status400BadRequest)
        {
        }

        public static void ValidateLatitude(double latitude)
        {
            if (latitude < -90 || latitude > 90)
                throw new LocationInvalidException();
        }

        public static void ValidateLongitude(double longitude)
        {
            if (longitude < -180 || longitude > 180)
                throw new LocationInvalidException();
        }
    }
}
