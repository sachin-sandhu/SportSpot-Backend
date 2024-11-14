namespace SportSpot.V1.Exceptions.Media
{
    public class MediaNotFoundException : AbstractSportSpotException
    {
        public MediaNotFoundException() : base("Media.NotFound", "Media not found", StatusCodes.Status404NotFound)
        {
        }
    }
}
