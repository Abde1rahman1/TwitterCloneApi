using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwitterCloneApi.Dtos;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountsController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration config;

		public AccountsController(UserManager<ApplicationUser> userManager, IConfiguration config)
		{
			_userManager = userManager;
			this.config = config;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(RegisterDto UserFromRequest)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser user = new ApplicationUser();
				user.UserName = UserFromRequest.Username;
				user.Gender = UserFromRequest.Gender;
				user.Bio = UserFromRequest.Bio;
				user.Birthday = UserFromRequest.Birthday;
				user.JoinTime = UserFromRequest.JoinTime;
				user.Location = UserFromRequest.Location;

				IdentityResult result = await _userManager.CreateAsync(user, UserFromRequest.Password);
				if (result.Succeeded)
				{
					return Ok(result);
				}
				foreach (var item in result.Errors)
				{
					ModelState.AddModelError("Password", item.Description);
				}

			}
			return BadRequest(ModelState);
		}
		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginDto userFromRequest)
		{
			// Check if the model is valid
			if (ModelState.IsValid)
			{
				ApplicationUser userFromDb = await _userManager.FindByNameAsync(userFromRequest.Username);
				if (userFromDb != null)
				{
					bool found = await _userManager.CheckPasswordAsync(userFromDb, userFromRequest.Password);
					if (found)
					{
						// Generate Token 
						List<Claim> UserClaims = new List<Claim>();
						UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, userFromDb.Id));
						UserClaims.Add(new Claim(ClaimTypes.Name, userFromDb.UserName));
						UserClaims.Add(new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // To make token random

						var UserRoles = await _userManager.GetRolesAsync(userFromDb);

						foreach (var rolename in UserRoles)
						{
							UserClaims.Add(new Claim(ClaimTypes.Role, rolename));
						}

						var SignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SecurityKey"]));
						SigningCredentials signingCredentials = new SigningCredentials(SignInKey, SecurityAlgorithms.HmacSha256);

						// Design Token
						JwtSecurityToken myToken = new JwtSecurityToken(
							issuer: config["JWT:IssuerIp"],  // Who created the token
							audience: config["JWT:AudienceIp"],  // Frontend
							expires: DateTime.Now.AddDays(30),
							claims: UserClaims,
							signingCredentials: signingCredentials
						);

						// Generate token response
						return Ok(new
						{
							token = new JwtSecurityTokenHandler().WriteToken(myToken),  // Convert token to string
							expiration = DateTime.Now.AddHours(1)
						});
					}
				}

				// Invalid username or password
				ModelState.AddModelError("Username", "Username or Password Invalid");
			}

			// Return bad request if model is not valid
			return BadRequest(ModelState);
		}

	}
}
