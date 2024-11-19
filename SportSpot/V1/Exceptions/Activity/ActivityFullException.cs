namespace SportSpot.V1.Exceptions.Activity
{
    public class ActivityFullException : AbstractSportSpotException
    {
        public ActivityFullException() : base("Activity.Full", "Activity is full.", StatusCodes.Status400BadRequest)
        {
        }
    }
}