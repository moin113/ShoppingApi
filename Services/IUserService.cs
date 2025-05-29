using MyntraClone.API.DTOs;

namespace MyntraClone.API.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<List<UserDto>> GetAllUsersAsync(); // Asynchronous method
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> AuthenticateAsync(string email, string password);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto); // ✅ NEW
        Task<bool> DeleteUserAsync(int id);                        // ✅ NEW
    }
}
