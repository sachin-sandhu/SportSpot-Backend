using Microsoft.AspNetCore.Identity;

namespace SportSpot.Exceptions.User
{
    public class UserRegisterException : AbstractSportSpotException
    {
        public IEnumerable<IdentityError> IdentityError { get; }

        public UserRegisterException(IEnumerable<IdentityError> error) : base("Error while register User")
        {
            IdentityError = error;
        }
    }
}
