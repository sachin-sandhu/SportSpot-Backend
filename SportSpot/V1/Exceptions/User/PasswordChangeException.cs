using Microsoft.AspNetCore.Identity;

namespace SportSpot.V1.Exceptions.User
{
    public class PasswordChangeException(IEnumerable<IdentityError> _error) : AbstractSportSpotException("", "Error while update User-Password", StatusCodes.Status400BadRequest)
    {
        public override List<ErrorResult> GetErrors()
        {
            return _error.Select(x => new ErrorResult { Code = $"User.{x.Code}", Message = x.Description }).ToList();
        }
    }
}
