using BaseLibrary;
using BaseLibrary.DTO;
using BaseLibrary.Response;
namespace Client.ApiClient.Interface
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(UserRegister user);
        Task<LoginResponse> SignInAsync(UserLogin user);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token);
        Task<WeatherForecast[]> GetWeatherForecasts();
    }
}
