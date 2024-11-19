namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityAlreadyJoinedException : AbstractSportSpotException
    {
        public ActivityAlreadyJoinedException() : base("Activity.AlreadyJoined", "The user has already joined the activity.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
