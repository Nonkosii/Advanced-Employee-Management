
using BaseLibrary.DTO;
using Client.ApiClient.Interface;

namespace Client.Helpers;

public class CustomHttpHandler(GetHttpClient getHttpClient, LocalStorage localStorage,
    IUserAccount userAccount) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        bool loginUrl = request.RequestUri.AbsoluteUri.Contains("login");
        bool registerUrl = request.RequestUri.AbsoluteUri.Contains("register");
        bool refreshTokenUrl = request.RequestUri.AbsoluteUri.Contains("refresh-token");

        if (loginUrl || registerUrl || refreshTokenUrl) return await base.SendAsync(request, cancellationToken);

        var result = await base.SendAsync(request, cancellationToken);

        if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Get Token from localStorage
            var stringToken = await localStorage.GetTokenAsync();
            if (stringToken is null) return result;

            // check if header contains token
            string token = string.Empty;
            try
            {
                token = request.Headers.Authorization!.Parameter!;
            }
            catch
            {

            }

            var deserializedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
            if (deserializedToken is null) return result;

            if (string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                return await base.SendAsync(request, cancellationToken);
            }

            // Call for Refresh Token
            var newJwtToken = await GetRefreshToken(deserializedToken.RefreshToken);
            if(string.IsNullOrEmpty(newJwtToken)) return result;

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newJwtToken);
            return await base.SendAsync(request, cancellationToken);

        }
        return result;
    }

    private async Task<string> GetRefreshToken(string refreshToken)
    {
        var result = await userAccount.RefreshTokenAsync(new RefreshTokenDto() { Token = refreshToken });
        string serializedToken = Serializations.SerializeObj(new UserSession() { Token = result.Token, RefreshToken = result.RefreshToken});
        await localStorage.SetTokenAsync(serializedToken);
        return result.Token;
    }
}