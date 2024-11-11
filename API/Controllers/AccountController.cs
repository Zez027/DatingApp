using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Interfaces;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
          private readonly DataContext _context;
          private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService,
          IMapper mapper)
          {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
          }
        
        [HttpPost("register")]   
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // Verificar se o modelo está válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Retorna os erros de validação
            }

            // Verificar se o usuário já existe
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512();
            user.UserName = registerDto.Username.ToLower();
            user.PassWordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PassWordSalt = hmac.Key;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
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
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(c => c.UserName == username.ToLower());
        }
        
    }
}