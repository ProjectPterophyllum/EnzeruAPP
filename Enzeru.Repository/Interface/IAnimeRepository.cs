using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnzeruAPP.Enzeru.Models;
namespace EnzeruAPP.Enzeru.Repository.Interface
{
    public interface IAnimeRepository
    {
        Task<int> InsertAnimeAsync(Anime anime);
        Task UpdateAnimeAsync(Anime anime);
        Task DeleteAnimeAsync(int id);
        Task<Anime?> GetAnimeByIdAsync(int id);
        Task<List<Anime>> GetAllAnimeAsync();
    }
}