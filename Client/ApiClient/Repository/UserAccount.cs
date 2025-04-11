using BaseLibrary.DTO;
using BaseLibrary.Response;
using Client.ApiClient.Interface;
using Client.Helpers;
using BaseLibrary;
using System.Net.Http.Json;

namespace Client.ApiClient.Repository
{
    public class UserAccount(GetHttpClient getHttpClient) : IUserAccount
    {
        public const string AuthUrl = "api/Auth";
        public async Task<GeneralResponse> CreateAsync(UserRegister user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);

            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error Occured");

            var response = await result.Content.ReadFromJsonAsync<GeneralResponse>();

            return response ?? new GeneralResponse(false, "Failed to parse register response from server");
        }

        public async Task<LoginResponse> SignInAsync(UserLogin user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);

            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error Occured");

            var response = await result.Content.ReadFromJsonAsync<LoginResponse>();

            return response ?? new LoginResponse(false, "Failed to parse login response from server");

        } 

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh-token", token);

            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error Occured");

            var response = await result.Content.ReadFromJsonAsync<LoginResponse>();

            return response ?? new LoginResponse(false, "Failed to parse register response from server");
        }

        public async Task<WeatherForecast[]> GetWeatherForecasts()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.GetFromJsonAsync<WeatherForecast[]>("api/weatherforecast");
            return result ?? [];
        }
    }
}
