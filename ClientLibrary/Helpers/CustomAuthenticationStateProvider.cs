

using BaseLibrary.DTO;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClientLibrary.Helpers
{
    public class CustomAuthenticationStateProvider(LocalStorage localStorage) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
           var stringToken = await localStorage.GetTokenAsync();

            if (string.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(anonymous));

            var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);

            if(deserializeToken is null) return await Task.FromResult(new AuthenticationState(anonymous));

            var getUserClaims = DecryptToken(deserializeToken.Token);
            if (getUserClaims is null) return await Task.FromResult(new AuthenticationState(anonymous));

            var claimsPrincipal = SetClaimPrincipal(getUserClaims);
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            var claimsPrincipal = new ClaimsPrincipal();

            if(userSession.Token is not null || userSession.RefreshToken is not null)
            {
                var serializeSession = Serializations.SerializeObj(userSession);
                                      await localStorage.SetTokenAsync(serializeSession);
                var getUserClaims = DecryptToken(userSession.Token ?? "");
                claimsPrincipal = SetClaimPrincipal(getUserClaims);
            }
            else
            {
                await localStorage.RemoveTokenAsync(); 
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private static CustomUserClaims DecryptToken(string jwtToken)
        {
            if(string.IsNullOrEmpty(jwtToken)) return new CustomUserClaims();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            var userId = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
            var name = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name)?.Value ?? ""; ;
            var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email)?.Value ?? ""; ;
            var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role)?.Value ?? ""; ;

            return new CustomUserClaims(userId, name, email, role);
        }

        private static ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();
            
            return new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new(ClaimTypes.NameIdentifier, claims.Id ?? ""),
                    new(ClaimTypes.Name, claims.Name ?? ""),
                    new(ClaimTypes.Email, claims.Email ?? ""),
                    new(ClaimTypes.Role, claims.Role ?? ""),

                ], "JwtAuth"));
        }
    }
}
