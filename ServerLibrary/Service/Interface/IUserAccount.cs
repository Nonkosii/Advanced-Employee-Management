using BaseLibrary.DTO;
using BaseLibrary.Response;

namespace ServerLibrary.Service.Interface
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(UserRegister user);
        Task<LoginResponse> SignInAsync(UserLogin user);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token);
    }
}
