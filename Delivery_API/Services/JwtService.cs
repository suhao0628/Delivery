using Delivery_API.Services.IServices;
using Delivery_Models.Models.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Delivery_API.Services
{
    public class JwtService: IJwtService
    {

        public readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtSecurityToken CreateToken(User user)
        {
            var jwtConfig = _configuration.GetSection("Jwt").Get<JwtConfigurations>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Identity
            var claimsList = new List<Claim>
                {
                    new Claim("UserId",user.Id.ToString()),
                };
            //Token
            var expires = DateTime.Now.AddMinutes(jwtConfig.Expires);
            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                audience: jwtConfig.Audience,
                claims: claimsList,
                notBefore: DateTime.Now,
                expires: expires,
                signingCredentials: credentials
            );
            return token;
        }
    }
}
