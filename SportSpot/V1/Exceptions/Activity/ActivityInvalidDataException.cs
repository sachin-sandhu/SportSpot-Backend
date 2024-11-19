namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityInvalidDataException : AbstractSportSpotException
    {
        public ActivityInvalidDataException() : base("Activity.InvalidDate", "Activity date is invalid.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
