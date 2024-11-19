namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityCreatorJoinException : AbstractSportSpotException
    {
        public ActivityCreatorJoinException() : base("Activity.Creator.Join", "The Creator can not join the activity.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
