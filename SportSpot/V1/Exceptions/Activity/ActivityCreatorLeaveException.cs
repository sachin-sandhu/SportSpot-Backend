namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityCreatorLeaveException : AbstractSportSpotException
    {
        public ActivityCreatorLeaveException() : base("Activity.CreatorLeave", "The creator of the activity cannot leave it.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
