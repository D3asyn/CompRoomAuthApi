using CompRoomAuthApi.DTOs;
using CompRoomAuthApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Win32;

namespace CompRoomAuthApi.Controllers
{
	public class AccountController(UserManager<ApplicationUser>userManager,RoleManager<IdentityRole> roleManager,IConfiguration config) : ControllerBase
	{
		[HttpPost("signup")]
		public async Task<IActionResult> SignUp(SignUp signUp)
		{
			var newAppUser = new ApplicationUser()
			{
				Email = signUp.Email,
				UserName = signUp.Email.Split("@")[0],
				PasswordHash = signUp.Password
			};

			var user = await userManager.FindByEmailAsync(signUp.Email);

			if(user is not null)
			{
				return BadRequest("User already exists");
			}

			var createUser = await userManager.CreateAsync(newAppUser, signUp.Password);

			var checkAdmin = await roleManager.RoleExistsAsync("Admin");

			if(!checkAdmin)
			{
				await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
				await userManager.AddToRoleAsync(newAppUser, "Admin");
				return Ok("Admin created");
			}

			var checkUser = await roleManager.RoleExistsAsync("User");
			if(!checkUser)
			{
				await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
			}
			await userManager.AddToRoleAsync(newAppUser, "User");
			return Ok("User created");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(Login loginDto)
		{
			if(loginDto is null)
			{
				return BadRequest("Empty input");
			}

			var user = await userManager.FindByNameAsync(loginDto.Username);

			if(user is null)
			{
				return NotFound("User not found");
			}

			bool checkpassword = await userManager.CheckPasswordAsync(user, loginDto.Password);

			if(!checkpassword)
			{
				return BadRequest("Invalid password");
			}

			var userRoles = await userManager.GetRolesAsync(user);

			string token = GenerateToken(user.Id, user.UserName!, user.Email!, userRoles.First());

			return Ok(token);

		}

		private string GenerateToken(string id, string name, string email, string role)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var userClaims = new[]
			{
				new Claim(ClaimTypes.Name, name),
				new Claim(ClaimTypes.Role, role),
				new Claim(ClaimTypes.NameIdentifier, id),
				new Claim(ClaimTypes.Email, email)
			};
			var token = new JwtSecurityToken(
				issuer: config["Jwt:Issuer"],
				audience: config["Jwt:Audience"],
				claims: userClaims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: credentials
			);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
