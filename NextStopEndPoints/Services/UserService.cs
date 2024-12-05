using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextStopEndPoints.Data;
using NextStopEndPoints.DTOs;
using NextStopEndPoints.Models;

namespace NextStopEndPoints.Services
{
    public class UserService : IUserService
    {
        private readonly NextStopDbContext _context;

        public UserService(NextStopDbContext context)
        {
            _context = context;
        }

        public async Task<UserDTO> GetUserById(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return null; 
                }

                return new UserDTO
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
            }
            catch (Exception ex)
            {              
                throw new Exception($"Error fetching user by ID: {ex.Message}");
            }
        }


        public async Task<UserDTO> GetUserByEmail(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return null;  
                }

                return new UserDTO
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user by email: {ex.Message}");
            }
        }


        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return users.Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Address,
                    Role = u.Role,
                    IsActive = u.IsActive
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all users: {ex.Message}");
            }
        }
        public async Task<UserDTO> CreateUser(CreateUserDTO userDTO)
        {
            try
            {
                if (await IsEmailUnique(userDTO.Email))
                {
                    var hashedPassword = PasswordHasher.HashPassword(userDTO.Password); 

                    var user = new User
                    {
                        Name = userDTO.Name,
                        Email = userDTO.Email,
                        PasswordHash = hashedPassword, 
                        Phone = userDTO.Phone,
                        Address = userDTO.Address,
                        Role = userDTO.Role,
                        IsActive = true
                    };

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    return new UserDTO
                    {
                        UserId = user.UserId,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Address = user.Address,
                        Role = user.Role,
                        IsActive = user.IsActive
                    };
                }

                throw new InvalidOperationException("Email already exists.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating user: {ex.Message}");
            }
        }

        public async Task<UserDTO> UpdateUser(int userId, UpdateUserDTO userDTO)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(userId);
                if (existingUser == null)
                {
                    return null;
                }

                existingUser.Name = userDTO.Name;
                existingUser.Phone = userDTO.Phone;
                existingUser.Address = userDTO.Address;
                existingUser.Role = userDTO.Role;
                existingUser.IsActive = userDTO.IsActive;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                return new UserDTO
                {
                    UserId = existingUser.UserId,
                    Name = existingUser.Name,
                    Email = existingUser.Email,
                    Phone = existingUser.Phone,
                    Address = existingUser.Address,
                    Role = existingUser.Role,
                    IsActive = existingUser.IsActive
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}");
            }
        }

        // Soft delete a user (set IsActive to false)
        public async Task<UserDTO> DeleteUser(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return null;
                }

                user.IsActive = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new UserDTO
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<UserDTO> ReactivateUser(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return null;
                }

                user.IsActive = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new UserDTO
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reactivating user: {ex.Message}");
            }
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            try
            {
                return !await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking email uniqueness: {ex.Message}");
            }
        }

        public async Task<UserDTO> GetUserByEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    return null; 
                }

                return new UserDTO
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address,
                    Role = user.Role,
                    IsActive = user.IsActive
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user by email and password: {ex.Message}");
            }
        }

        public async Task ResetEmail(int userId, string newEmail)
        {
            try
            {
                if (!await IsEmailUnique(newEmail))
                {
                    throw new InvalidOperationException("The new email is already in use by another user.");
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return; 
                }

                user.Email = newEmail;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error resetting email: {ex.Message}");
            }
        }


        public async Task ResetPassword(int userId, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return;  
                }

                var hashedPassword = PasswordHasher.HashPassword(newPassword);
                user.PasswordHash = hashedPassword;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error resetting password: {ex.Message}");
            }
        }

    }
}
