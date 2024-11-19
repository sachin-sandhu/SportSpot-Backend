namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityExpiredException : AbstractSportSpotException
    {
        public ActivityExpiredException() : base("Activity.Expired", "Activity is expired.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
