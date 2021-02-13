using System.Collections.Generic;
using System.Threading.Tasks;
using LoginApp.Models;

namespace LoginApp.Services
{
    public interface IUserService
    {
        Task<User> GetUserAsync(int id);
        Task<List<User>> GetUsersAsync();
        Task AddUserAsync(User user);
    }
}