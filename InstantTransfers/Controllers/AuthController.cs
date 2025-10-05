using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InstantTransfers.DTOs.User;
using InstantTransfers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace InstantTransfers.Controllers
{

    [Route("api/auth")]
    [ApiController]
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        private static readonly List<User> _users = new();

        [HttpPost("register")]
        public ActionResult<User> Register(UserRegisterDto request)
        {

            var newUser = new User
            {
                UserName = request.Username,
                Email = request.Email
            };

            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(newUser, request.Password);

            newUser.PasswordHash = hashedPassword;

            _users.Add(newUser);

            return Ok(newUser);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserLoginDto request)
        {
            User? user = _users.FirstOrDefault(u => u.UserName == request.Username);
            if (user == null)
                return BadRequest("User not found.");

            if (new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return BadRequest("Wrong password.");
            }

            return Ok("Login successful. token: xxx");
        }


        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
