using System.Data.SQLite;
using EnzeruAPP.Enzeru.Models;
using EnzeruAPP.Enzeru.Repository.Interface;
namespace EnzeruAPP.Enzeru.Repository.Classes
{
    public class UserAnimeListRepository : IUserAnimeListRepository
    {
        public async Task<int> AddAnimeToUserListAsync(int userId, int animeId, bool status)
        {
            var query = "INSERT INTO UserAnimeList (UserID, AnimeID, Status) VALUES (@UserID, @AnimeID, @Status); SELECT last_insert_rowid();";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@AnimeID", animeId);
            command.Parameters.AddWithValue("@Status", status);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task UpdateUserAnimeStatusAsync(int id, bool status)
        {
            var query = "UPDATE UserAnimeList SET Status = @Status WHERE ID = @ID";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@Status", status);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteUserAnimeAsync(int id)
        {
            var query = "DELETE FROM UserAnimeList WHERE ID = @ID";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<UserAnimeList>> GetUserAnimeListAsync(int userId)
        {
            var query = "SELECT * FROM UserAnimeList WHERE UserID = @UserID";
            var userAnimeList = new List<UserAnimeList>();

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                userAnimeList.Add(new UserAnimeList
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    AnimeId = reader.GetInt32(2),
                    Status = reader.GetBoolean(3),
                });
            }
            return userAnimeList;
        }
    }
}