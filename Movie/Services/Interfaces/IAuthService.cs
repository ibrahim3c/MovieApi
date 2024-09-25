using Movie.DTOs;
using Movie.Models;

namespace Movie.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(UserRegisterationDTO userRegisterationDTO);
        Task<AuthResult> LoginAsync(UserLoginDTO userLoginDTO);
        Task<RoleResult> AddRoleToUserAsync(AddRoleToUserDTO addRoleDTO);
        Task<RoleResult> CreateRoleAsync(string roleName);
        Task<List<AppUser>> GetUsersAsync();
        Task<List<AppRole>> GetAllRoles();
        Task<AuthResult> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<string> GetRefreshTokenOfUser(string email);



    }
}
