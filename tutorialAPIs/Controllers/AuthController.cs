using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace tutorialAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public AuthController(DataContext context)
        {
            _dataContext = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> SignUp(UserDto request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);


            if (user != null)
            {
                return BadRequest("User already with this username");
            }

            var newUser = new User
            {
                UserName = request.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            _dataContext.Users.Add(newUser);
            await _dataContext.SaveChangesAsync();

            var token = GenerateJwtToken(newUser.Id, newUser.UserName);
            return new AuthResponse { Token = token };
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(UserDto request)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("User not found OR Incorrect Password");
            }

            var token = GenerateJwtToken(user.Id, user.UserName);
            return new AuthResponse { Token = token };
        }

        private string GenerateJwtToken(int userId, string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GenerateSecurityKey(32));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateSecurityKey(int size)
        {
            var randomNumberGenerator = RandomNumberGenerator.Create();
            var data = new byte[size];
            randomNumberGenerator.GetBytes(data);
            return Convert.ToBase64String(data);
        }
    }
}
