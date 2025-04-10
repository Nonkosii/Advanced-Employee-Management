using BaseLibrary;
using BaseLibrary.DTO;
using BaseLibrary.Response;
namespace ClientLibrary.ApiClient.Interface
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token);
        Task<WeatherForecast[]> GetWeatherForecasts();
    }
}
