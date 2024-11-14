using SportSpot.V1.User.Dtos;
using SportSpot.V1.User.Dtos.Auth;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Mapper
{
    public static class AuthUserMapper
    {
        public static AuthUserResponseDto ConvertToDto(this AuthUserEntity entity, AuthUserEntity? sender = null)
        {
            AuthUserResponseDto userResponseDto = new()
            {
                Id = entity.Id,
                Username = entity.UserName ?? string.Empty,
                Biography = entity.Biography,
                Avatar = $"http://localhost:8080/user/{entity.Id}/avatar",
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.DateOfBirth,
            };
            if (sender != null && entity.Id == sender.Id)
                userResponseDto.Email = entity.Email;
            return userResponseDto;
        }
    }
}
