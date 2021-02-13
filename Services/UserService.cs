using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoginApp.Models;
using Newtonsoft.Json;

namespace LoginApp.Services
{
    public class UserService
    {
        public async Task AddUserAsync(User user)
        {
            var users = await this.GetUsersAsync();
            if (await this.FindUserAsync(user.Id) == null)
                users.Add(user);

            await System.IO.File.WriteAllTextAsync("./users.json", JsonConvert.SerializeObject(users));
        }

        private async Task<User> FindUserAsync(int id)
        {
            var users = await this.GetUsersAsync();
            var user = users.FirstOrDefault(u => u.Id == id);

            return user;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var usersStore = await System.IO.File.ReadAllTextAsync("./users.json");
            var users = JsonConvert.DeserializeObject<List<User>>(usersStore);

            return users;
        }

        public async Task<int> GetLastId()
        {
            var users = await this.GetUsersAsync();
            var user = users.OrderBy(u => u.Id);

            return user.Last().Id;
        }
    }
}
