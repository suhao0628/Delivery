using AutoMapper;
using Delivery_API.Data;
using Delivery_API.Exceptions;
using Delivery_API.Services.IServices;
using Delivery_Models.Models;
using Delivery_Models.Models.Dto;
using Delivery_Models.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery_API.Services
{
    public class UserService: IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        public UserService(AppDbContext context, IMapper mapper, IJwtService jwtService)
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
        }
        public async Task<TokenResponse> Register(UserRegisterModel register)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == register.Email);
            if(user!=null)
            {
                throw new BadRequestException(register.Email + " is already taken");
            }
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
                throw new NotFoundException("User does not exist");
            }
            if (user.Password != login.Password)
            {
                throw new BadRequestException("Invaild Password");
            }
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(_jwtService.CreateToken(user))
            };
        }
        
        public async Task<UserDto> GetProfile(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User does not exist");
            }
            return _mapper.Map<UserDto>(user);
        }
        public async Task EditProfile(UserEditModel profile, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User does not exist");
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
