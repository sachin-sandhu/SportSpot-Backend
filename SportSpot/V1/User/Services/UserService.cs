using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using SportSpot.V1.Exceptions.User;
using SportSpot.V1.Extensions;
using SportSpot.V1.Media.Entities;
using SportSpot.V1.Media.Services;
using SportSpot.V1.User.Context;
using SportSpot.V1.User.Dtos;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.User.Services
{
    public class UserService(UserManager<AuthUserEntity> _userManager, IMediaService _mediaService, AuthContext _dbContext) : IUserService
    {
        public async Task<byte[]> GetAvatar(AuthUserEntity user)
        {
            if (user.AvatarId == null)
                return []; //TODO: return default avatar
            MediaEntity media = await _mediaService.GetMedia(user.AvatarId.Value);
            return await _mediaService.GetMediaAsBytes(media);
        }
        public async Task<AuthUserEntity> GetUser(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException();
        }

        public async Task Update(UpdateUserDto updateUserDto, AuthUserEntity authUser)
        {
            using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(CancellationToken.None);
            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                IdentityResult changeResult = await _userManager.SetEmailAsync(authUser, updateUserDto.Email);
                if (!changeResult.Succeeded)
                    throw new EmailChangeException(changeResult.Errors);
            }

            if (!string.IsNullOrEmpty(updateUserDto.Username))
            {
                IdentityResult changeResult = await _userManager.SetUserNameAsync(authUser, updateUserDto.Username);
                if (!changeResult.Succeeded)
                    throw new UsernameChangeException(changeResult.Errors);
            }

            if (updateUserDto.Password != null && !authUser.IsOAuth)
            {
                IdentityResult changeResult = await _userManager.ChangePasswordAsync(authUser, updateUserDto.Password.OldPassword, updateUserDto.Password.NewPassword);
                if (!changeResult.Succeeded)
                    throw new PasswordChangeException(changeResult.Errors);
            }

            if (!string.IsNullOrEmpty(updateUserDto.Biography))
                authUser.Biography = updateUserDto.Biography;
            if (!string.IsNullOrEmpty(updateUserDto.LastName))
                authUser.LastName = updateUserDto.LastName;
            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
                authUser.FirstName = updateUserDto.FirstName;

            if (updateUserDto.DateOfBirth != null)
            {
                DateTime date = updateUserDto.DateOfBirth.Value.ToUniversalTime();
                authUser.DateOfBirth = new(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
                if (authUser.DateOfBirth > DateTime.UtcNow)
                    throw new InvalidBirthDateException();
            }

            if (updateUserDto.AvatarAsBase64 != null)
            {
                MediaEntity media = await _mediaService.CreateMedia("User_Avatar", updateUserDto.AvatarAsBase64.GetAsByteImage(), authUser);
                authUser.AvatarId = media.Id;
            }

            await _userManager.UpdateAsync(authUser);
            await transaction.CommitAsync();
        }
    }
}
