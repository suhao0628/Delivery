using Delivery_Models.Models.Entity;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery_API.Services.IServices
{
    public interface IJwtService
    {
        JwtSecurityToken CreateToken(User user);
    }
}
