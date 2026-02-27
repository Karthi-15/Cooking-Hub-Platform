using System.Text.Json;
using dotnetapp.Data;
using dotnetapp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace dotnetapp.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<(int, string)> Registration(User model, string role)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return (0, "Email is required");

            var normalizedEmail = model.Email.ToUpper();
            var userExists = await _context.Users.SingleOrDefaultAsync(u => u.Email.ToUpper() == normalizedEmail);

            if (userExists != null)
                return (0, "User already exists");

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Username,
                PhoneNumber = model.MobileNumber,
                Name = model.Username
            };
            // Console.WriteLine("from frontend password: "+model.Password);
            string decryptedPassword = AESHelper.Decrypt(model.Password);
            // Console.WriteLine("from backend encryption: "+decryptedPassword);
            var result = await _userManager.CreateAsync(user, decryptedPassword);


            // var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (0, "User creation failed! Please check user details and try again");

            if (!await _roleManager.RoleExistsAsync(role))
                return (0, $"Role '{role}' does not exist. Please create the role first.");

            await _userManager.AddToRoleAsync(user, role);
            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            return (1, "User created successfully!");
        }

        public async Task<(int, string)> Login(LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return (0, "Email is required");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (0, "Invalid email");
            // Console.WriteLine($"encrypted password from frotnend: {model.Password}");
            string decryptedPassword = AESHelper.Decrypt(model.Password);
// Console.WriteLine($"Decrypted password: {decryptedPassword}");
    
            if (!await _userManager.CheckPasswordAsync(user, decryptedPassword))
                return (0, "Invalid password");

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, userRoles[0])
            };

            var token = GenerateToken(authClaims);

// 🔹 Fetch recently saved user data from User Table
    var storedUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
    if (storedUser == null)
        return (0, "User record not found in database");

    //  Serialize Response with Token, User Details, and Role(s)
    var responseObject = new
    {
        token,
        user = new
        {
            id = storedUser.UserId,  // Fetching user ID from DB
            username = storedUser.Username,
            email = storedUser.Email,
            mobileNumber = storedUser.MobileNumber
        },
        role = userRoles[0]
    };

    string jsonResponse = JsonSerializer.Serialize(responseObject);

    return (1, jsonResponse);
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            try
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:ValidIssuer"],
                    audience: _configuration["Jwt:ValidAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token generation failed: {ex.Message}");
                return null;
            }
        }
    }
}