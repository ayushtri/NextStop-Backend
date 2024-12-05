using NextStopEndPoints.DTOs;

namespace NextStopEndPoints.Services
{
    public interface IUserService
    {
        Task<UserDTO> GetUserById(int userId);
        Task<UserDTO> GetUserByEmail(string email);
        Task<IEnumerable<UserDTO>> GetAllUsers();
        Task<UserDTO> CreateUser(CreateUserDTO userDTO);
        Task<UserDTO> UpdateUser(int userId, UpdateUserDTO userDTO);
        Task<UserDTO> DeleteUser(int userId);
        Task<UserDTO> ReactivateUser(int userId);
        Task<bool> IsEmailUnique(string email);
        Task<UserDTO> GetUserByEmailAndPassword(string email, string password);
        Task ResetEmail(int userId, string newEmail);
        Task ResetPassword(int userId, string newPassword);
    }
}
