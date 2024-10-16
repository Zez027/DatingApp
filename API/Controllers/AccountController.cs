using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
          private readonly DataContext _context;
          private readonly ITokenService _tokenService;

          public AccountController(DataContext context, ITokenService tokenService)
          {
            _tokenService = tokenService;
            _context = context;
          }
        
        [HttpPost("register")] // POST: api/account/register?username=dave&password=pwd
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PassWordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PassWordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
                //PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user =  await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(c => c.UserName == loginDto.Username);

            if(user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PassWordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != user.PassWordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(c => c.UserName == username.ToLower());
        }
        
    }
}