using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnzeruAPP.Enzeru.Repository.Interface
{
    public interface IUserRepository
    {
        Task<int> InsertUserAsync(User user);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}