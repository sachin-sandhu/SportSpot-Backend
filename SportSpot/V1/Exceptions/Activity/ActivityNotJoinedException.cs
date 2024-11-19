namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityNotJoinedException : AbstractSportSpotException
    {
        public ActivityNotJoinedException() : base("Activity.NotJoined", "The user is not joined to the activity.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
