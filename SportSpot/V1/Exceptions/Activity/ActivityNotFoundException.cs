namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityNotFoundException : AbstractSportSpotException
    {
        public ActivityNotFoundException() : base("Activity.NotFound", "Activity not found.", StatusCodes.Status404NotFound)
        {
        }
    }
}
