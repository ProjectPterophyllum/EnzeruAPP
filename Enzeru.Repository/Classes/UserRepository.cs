using System.Data.SQLite;
using EnzeruAPP.Enzeru.Models;
using EnzeruAPP.Enzeru.Repository.Interface;
namespace EnzeruAPP.Enzeru.Repository.Classes
{
    public class UserRepository : IUserRepository
    {
        public async Task<int> InsertUserAsync(User user)
        {
            var query = "INSERT INTO User (Username) VALUES (@Username); SELECT last_insert_rowid();";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            var query = "SELECT * FROM User WHERE ID = @ID";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    ID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                };
            }
            return null;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var query = "SELECT * FROM User WHERE Username = @Username";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    ID = reader.GetInt32(0),
                    Username = reader.GetString(1),
                };
            }
            return null;
        }
    }
}