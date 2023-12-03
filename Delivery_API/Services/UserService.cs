using AutoMapper;
using Azure.Core;
using Delivery_API.Data;
using Delivery_API.Exceptions;
using Delivery_API.Services.IServices;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Entity;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery_API.Services
{
    public class UserService: IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IDistributedCache _cache;
        private readonly IOptions<JwtConfigurations> _jwtOptions;

        public UserService(AppDbContext context, IMapper mapper, IJwtService jwtService, IDistributedCache cache, IOptions<JwtConfigurations> jwtOptions)
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
            _cache = cache;
            _jwtOptions = jwtOptions;  
        }
        public bool IsUniqueUser(UserRegisterModel register)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == register.Email);
            if (user == null)
            {
                return true;
            }
            return false;
        }
        public async Task<TokenResponse> Register(UserRegisterModel register)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == register.Email);
            var registerUser = _mapper.Map<User>(register);

            await _context.Users.AddAsync(registerUser);
            await _context.SaveChangesAsync();

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(_jwtService.CreateToken(registerUser))
            };
        }
        public async Task<TokenResponse> Login(LoginCredentials login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == login.Email);

            if (user == null)
            {
                throw new BadRequestException(new Response
                {
                    Message = "User does not exist. Login failed"
                });
            }
            if (user.Password != login.Password)
            {
                throw new BadRequestException(new Response
                {
                    Message = "Incorrect Password. Login failed"
                });
            }
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(_jwtService.CreateToken(user))
            };
        }

        public async Task Logout(string token)
        {
            
            await _cache.SetStringAsync(token,
                " ", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(_jwtOptions.Value.Expires)
                    //AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_jwtOptions.Value.Expires)
                });
        }
        public async Task<bool> IsActiveToken(string token)
        {
            return await _cache.GetStringAsync(token) == null;
        }
        public async Task<UserDto> GetProfile(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(new Response
                {
                    Message = "User does not exist"
                });
            }
            return _mapper.Map<UserDto>(user);
        }
        public async Task EditProfile(UserEditModel profile, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new NotFoundException(new Response
                {
                    Message = "User does not exist"
                });
            }

            user.FullName = profile.FullName;
            user.BirthDate = profile.BirthDate;
            user.Gender = profile.Gender;
            user.Address = profile.Address;
            user.PhoneNumber = profile.PhoneNumber;

             _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
