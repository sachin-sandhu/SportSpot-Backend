namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityTooManyTagsException : AbstractSportSpotException
    {
        public ActivityTooManyTagsException() : base("Activity.TooManyTags", "Too many tags!", StatusCodes.Status400BadRequest)
        {
        }
    }
}
