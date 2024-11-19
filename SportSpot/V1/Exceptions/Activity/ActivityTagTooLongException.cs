namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityTagTooLongException : AbstractSportSpotException
    {
        public ActivityTagTooLongException() : base("Activity.TagTooLong", "Activity Tag is too long.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
