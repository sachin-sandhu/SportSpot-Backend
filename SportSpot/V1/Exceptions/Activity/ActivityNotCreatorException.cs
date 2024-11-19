namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityNotCreatorException : AbstractSportSpotException
    {
        public ActivityNotCreatorException() : base("Activity.NotCreator", "The user is not the creator of the activity.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
