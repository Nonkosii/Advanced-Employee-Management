using BaseLibrary.DTO;
using BaseLibrary.Response;

namespace ServerLibrary.Service.Interface
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto token);
    }
}
