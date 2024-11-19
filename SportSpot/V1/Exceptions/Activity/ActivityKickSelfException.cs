namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityKickSelfException : AbstractSportSpotException
    {
        public ActivityKickSelfException() : base("Activity.Kick.Self", "You can not kick yourself from the activity.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
