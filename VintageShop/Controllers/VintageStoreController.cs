using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VintageStore.Context;
using VintageStore.Helpers;
using VintageStore.Model;

namespace VintageStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VintageStoreController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public VintageStoreController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }
            var user = await _authContext.Users.
                FirstOrDefaultAsync(x => x.Username == userObj.Username);
            if (user == null)
            {
                return NotFound(new { Message = "User Not Found" });
            }
            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is incorrect" });
            }

            user.Token = CreateJwt(user);
            return Ok(new
            {
                Token= user.Token,
                Message = "Login Sucess!"
            });
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }
            //Checking username 
            if (await CheckUserNameExistAsync(userObj.Username))
                return BadRequest(new { Message = "Username Already Exist" });

            userObj.Password = PasswordHasher.HashPassword(userObj.Password);

            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "User is registered!"
            });
        }
        private Task<bool> CheckUserNameExistAsync(string username)
        => _authContext.Users.AnyAsync(x => x.Username == username);


        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {

                new Claim(ClaimTypes.Name,$"{user.Username}")
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDesciptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(10),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDesciptor);
            return jwtTokenHandler.WriteToken(token);
        }

        
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }








    }
}

