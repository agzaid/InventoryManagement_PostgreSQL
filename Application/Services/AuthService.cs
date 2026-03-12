using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Contracts.Persistance;
using Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using Identity.Identity;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApplicationUser?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = await _unitOfWork.UserRepository.GetByUsernameAsync(username);
            if (user == null || !user.IsActive)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            // Note: Update will be handled by ASP.NET Identity UserManager
            // await _unitOfWork.UserRepository.UpdateAsync(user);
            // await _unitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _unitOfWork.UserRepository.GetByEmailAsync(userId); // Since we're using email as identifier
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _unitOfWork.UserRepository.GetByEmailAsync(userId);
            if (user == null)
                return new List<string>();

            // For now, return empty list - will be implemented with ASP.NET Identity UserManager
            // TODO: Implement with UserManager.GetRolesAsync(user)
            return new List<string>();
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            var userRoles = await GetUserRolesAsync(userId);
            return userRoles.Contains(roleName);
        }

        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with PBKDF2
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            // Combine salt and hash for storage
            byte[] combined = new byte[salt.Length + Convert.FromBase64String(hashed).Length];
            Array.Copy(salt, 0, combined, 0, salt.Length);
            Array.Copy(Convert.FromBase64String(hashed), 0, combined, salt.Length, Convert.FromBase64String(hashed).Length);

            return Convert.ToBase64String(combined);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                byte[] combined = Convert.FromBase64String(hash);
                
                // Extract salt (first 16 bytes)
                byte[] salt = new byte[16];
                Array.Copy(combined, 0, salt, 0, 16);
                
                // Extract hash (remaining bytes)
                byte[] storedHash = new byte[combined.Length - 16];
                Array.Copy(combined, 16, storedHash, 0, storedHash.Length);
                
                // Hash the provided password with the extracted salt
                string computedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
                
                byte[] computedHashBytes = Convert.FromBase64String(computedHash);
                
                // Compare the hashes
                return computedHashBytes.SequenceEqual(storedHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
