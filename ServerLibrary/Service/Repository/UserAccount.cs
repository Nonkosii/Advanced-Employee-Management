using BaseLibrary.DTO;
using BaseLibrary.Model;
using BaseLibrary.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Configurations;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ServerLibrary.Service.Repository
{
    public class UserAccount(IOptions<JwtSection> config, AppDBContext appDBContext) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(UserRegister user)
        {
            if (user == null) return new GeneralResponse(false, "Model is empty.");

            var checkUser = await FindUserByEmail(user.Email);
            if (checkUser == null) return new GeneralResponse(false, "User already registered");

            var applicationUser = await AddToDatabase(new ApplicationUser()
            {
                FullName = user.FullName,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });

            // Check and assign admin role
            var checkAdminRole = await appDBContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name.Equals(Constants.Admin));
            if (checkAdminRole == null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
                await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserIdId = applicationUser.Id });
                return new GeneralResponse(true, "Account created");
            }

            // Check and assign user role
            var checkUserRole = await appDBContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole == null)
            {
                response = await AddToDatabase(new SystemRole() { Name = Constants.User });
                await AddToDatabase(new UserRole() { RoleId = response.Id, UserIdId = applicationUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserIdId = applicationUser.Id });
            }
            return new GeneralResponse(true, "Account created");
        }
        public async Task<LoginResponse> SignInAsync(UserLogin user)
        {
            if (user == null) return new LoginResponse(false, "Model is empty.");
            var applicationUser = await FindUserByEmail(user.Email);
            if (applicationUser == null || string.IsNullOrEmpty(applicationUser.Password) ||
                !BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
            {
                return new LoginResponse(false, "Email/Password not valid");
            }

            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole == null) return new LoginResponse(false, "User role not found");


            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getUserRole == null) return new LoginResponse(false, "User role name not found");

            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name);
            string refreshToken = GenerateRefreshToken();

            var findUser = await appDBContext.RefreshTokens.FirstOrDefaultAsync(_ => _.UserId == applicationUser.Id);
            if (findUser is not null)
            {
                findUser.Token = refreshToken;
                await appDBContext.SaveChangesAsync();
                
            }
            else
            {
                await AddToDatabase(new RefreshToken() { Token = refreshToken, UserId = applicationUser.Id });
            }
            return new LoginResponse(true, "Login successful", jwtToken, refreshToken);
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role),

            };
            var token = new JwtSecurityToken(
                issuer: config.Value.Issuer,
                audience: config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserRole?> FindUserRole(int userId) =>
            await appDBContext.UserRoles.FirstOrDefaultAsync(_ => _.UserIdId == userId);

        private async Task<SystemRole?> FindRoleName(int roleId) =>
            await appDBContext.SystemRoles.FirstOrDefaultAsync(_ => _.Id == roleId);
        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        private async Task<ApplicationUser?> FindUserByEmail(string email) =>
                await appDBContext.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));

        private async Task<T> AddToDatabase<T>(T model) where T : notnull
        {
            var result = appDBContext.Add(model);
            await appDBContext.SaveChangesAsync();
            return (T)result.Entity;
        }


        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token)
        {
            if (token == null) return new LoginResponse(false, "Model is empty");

            var findToken = await appDBContext.RefreshTokens.FirstOrDefaultAsync(_ => _.Token.Equals(token.Token));
            if (findToken == null) return new LoginResponse(false, "Refresh token is required");

            var user = await appDBContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Id.Equals(findToken.UserId));
            if (user == null) return new LoginResponse(false, "Refresh token could not be generated because user is not found.");

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole!.RoleId);
            string jwtToken = GenerateToken(user, roleName!.Name);
            string refreshToken = GenerateRefreshToken();

            var updatedRefreshToken = await appDBContext.RefreshTokens.FirstOrDefaultAsync(_ => _.UserId == user.Id);
            if (updatedRefreshToken == null) return new LoginResponse(false, "Refresh token could not be generated because user has not signed in.");

            updatedRefreshToken.Token = refreshToken;
            await appDBContext.SaveChangesAsync();
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
        }
    }
}
