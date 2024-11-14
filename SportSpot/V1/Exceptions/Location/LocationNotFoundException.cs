namespace SportSpot.V1.Exceptions.Location
{
    public class LocationNotFoundException : AbstractSportSpotException
    {
        public LocationNotFoundException() : base("Location.NotFound", "Location not found", StatusCodes.Status404NotFound)
        {
        }
    }
}
