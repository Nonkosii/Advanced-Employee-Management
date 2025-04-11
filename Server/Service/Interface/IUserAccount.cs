using BaseLibrary.DTO;
using BaseLibrary.Response;

namespace Server.Service.Interface
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(UserRegister user);
        Task<LoginResponse> SignInAsync(UserLogin user);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token);
    }
}
