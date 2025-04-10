using BaseLibrary.DTO;
using System.Net.Http;

namespace ClientLibrary.Helpers;

public class GetHttpClient(IHttpClientFactory httpClientFactory, LocalStorage localStorage)
{
    private const string HeaderKey = "Authorization";

    public async Task<HttpClient> GetPrivateHttpClient()
    {
        var client = httpClientFactory.CreateClient("SystemApiClient");
        var stringToken = await localStorage.GetTokenAsync();

        if (string.IsNullOrEmpty(stringToken)) return client;

        var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);

        if(deserializeToken == null) return client;

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializeToken.Token);
        return client;
    }

    public HttpClient GetPublicHttpClient()
    {
        var client = httpClientFactory.CreateClient("SystemApiClient");
        client.DefaultRequestHeaders.Remove(HeaderKey);
        return client;
    }
}
