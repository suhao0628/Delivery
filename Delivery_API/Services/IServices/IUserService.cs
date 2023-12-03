using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_API.Services.IServices
{
    public interface IUserService
    {
        bool IsUniqueUser(UserRegisterModel register);
        Task<TokenResponse> Register(UserRegisterModel register);
        Task<TokenResponse> Login(LoginCredentials credentials);

        Task Logout(string token);

        Task<bool> IsActiveToken(string token);

        Task<UserDto> GetProfile(Guid userId);
        Task EditProfile(UserEditModel userEditModel, Guid userId);
    }
}
