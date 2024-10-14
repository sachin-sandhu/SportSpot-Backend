using Microsoft.AspNetCore.Identity;

namespace SportSpot.V1.Exceptions
{
    public class UserRegisterException(IEnumerable<IdentityError> _error) : AbstractSportSpotException("", "Error while register User", StatusCodes.Status400BadRequest)
    {
        public override List<ErrorResult> GetErrors()
        {
            return _error.Select(x => new ErrorResult { Code = $"User.{x.Code}", Message = x.Description }).ToList();
        }
    }
}
