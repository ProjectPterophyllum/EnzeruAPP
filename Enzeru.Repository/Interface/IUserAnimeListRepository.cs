using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnzeruAPP.Enzeru.Repository.Interface
{
    public interface IUserAnimeListRepository
    {
        Task<int> AddAnimeToUserListAsync(int userId, int animeId, bool status);
        Task UpdateUserAnimeStatusAsync(int id, bool status);
        Task DeleteUserAnimeAsync(int id);
        Task<List<UserAnimeList>> GetUserAnimeListAsync(int userId);
    }
}