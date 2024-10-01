using Microsoft.AspNetCore.Identity;

namespace SportSpot.V1.Exceptions.User
{
    public class UserRegisterException(IEnumerable<IdentityError> _error) : AbstractSportSpotException("", "Error while register User", StatusCodes.Status400BadRequest)
    {
        public override List<ErrorResult> GetErrors()
        {
            return _error.Select(x => new ErrorResult { Code = x.Code, Message = x.Description }).ToList();
        }
    }
}
