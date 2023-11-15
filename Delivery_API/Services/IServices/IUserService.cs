using Delivery_API.Models;
using Delivery_API.Models.Dto;

namespace Delivery_API.Services.IServices
{
    public interface IUserService
    {
        Task<TokenResponse> Register(UserRegisterModel register);
        Task<TokenResponse> Login(LoginCredentials credentials);

        Task<UserDto> GetProfile(Guid userId);
        Task EditProfile(UserEditModel userEditModel, Guid userId);
    }
}
