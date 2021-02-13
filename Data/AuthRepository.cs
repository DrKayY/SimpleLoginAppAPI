using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginApp.Models;
using LoginApp.Services;

namespace LoginApp.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserService _userService;
        public AuthRepository(UserService userService)
        {
            _userService = userService;
        }
        public async Task<User> Login(string username, string password)
        {
            var users = await _userService.GetUsersAsync();

            var user = users.Find(u => u.Name == username);
            
            if (user == null)
                return null;

            if (!VerifiedPasswordHash(password, CreateByteArrayFromString(user.PasswordHash), CreateByteArrayFromString(user.PasswordSalt)))
                return null;
            
            return user;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            var lastId = _userService.GetLastId();
            user.PasswordHash = CreateStringFromByteArray(passwordHash);
            user.PasswordSalt = CreateStringFromByteArray(passwordSalt);
            user.Id = await lastId + 1;

            await _userService.AddUserAsync(user);

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            var users = await _userService.GetUsersAsync();
            var user = users.Find(u => u.Name == username);
            if (user != null)
                return true;
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()
            )
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifiedPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)
            )
            {
                var verifyingHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != verifyingHash[i]) return false;
                }
            }
            
            return true;
        }
        private static string CreateStringFromByteArray(byte[] hash)
        {
            var sBuilder = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sBuilder.Append(hash[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private static byte[] CreateByteArrayFromString(string hexString) 
        {
            return Enumerable.Range(0, hexString.Length)
                            .Where(x => x % 2 == 0)
                            .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                            .ToArray();
        }
    }
}