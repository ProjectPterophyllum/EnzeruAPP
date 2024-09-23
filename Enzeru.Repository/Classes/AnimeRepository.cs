using System.Data.SQLite;
using EnzeruAPP.Enzeru.Models;
using EnzeruAPP.Enzeru.Repository.Interface;

namespace EnzeruAPP.Enzeru.Repository.Classes
{
    public class AnimeRepository : IAnimeRepository
    {
        public async Task<int> InsertAnimeAsync(Anime anime)
        {
            var query = "INSERT INTO Anime (Title, Rating, Description, Type, Genre, ImageURL, ReleaseDate, Url) " +
                        "VALUES (@Title,@Rating, @Description, @Type, @Genre, @ImageURL, @ReleaseDate, @Url); SELECT last_insert_rowid();";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@Title", anime.Title);
            command.Parameters.AddWithValue("@Rating", anime.Rating);
            command.Parameters.AddWithValue("@Description", anime.Description);
            command.Parameters.AddWithValue("@Type", anime.Type);
            command.Parameters.AddWithValue("@Genre", anime.Genre);
            command.Parameters.AddWithValue("@ImageURL", anime.ImageURL);
            command.Parameters.AddWithValue("@ReleaseDate", anime.ReleaseDate);
            command.Parameters.AddWithValue("@Url", anime.Url);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task UpdateAnimeAsync(Anime anime)
        {
            var query = "UPDATE Anime SET Title = @Title, Rating = @Rating, Description = @Description, Type = @Type, " +
                        "Genre = @Genre, ImageURL = @ImageURL, ReleaseDate = @ReleaseDate, Url = @Url WHERE ID = @ID";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@ID", anime.ID);
            command.Parameters.AddWithValue("@Title", anime.Title);
            command.Parameters.AddWithValue("@Rating", anime.Rating);
            command.Parameters.AddWithValue("@Description", anime.Description);
            command.Parameters.AddWithValue("@Type", anime.Type);
            command.Parameters.AddWithValue("@Genre", anime.Genre);
            command.Parameters.AddWithValue("@ImageURL", anime.ImageURL);
            command.Parameters.AddWithValue("@ReleaseDate", anime.ReleaseDate);
            command.Parameters.AddWithValue("@Url", anime.Url);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAnimeAsync(int id)
        {
            var query = "DELETE FROM Anime WHERE ID = @ID";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<Anime?> GetAnimeByIdAsync(int id)
        {
            var query = "SELECT * FROM Anime WHERE ID = @ID";

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Anime
                {
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Rating = reader.GetString(2),
                    Description = reader.GetString(3),
                    Type = reader.GetString(4),
                    Genre = reader.GetString(5),
                    ImageURL = reader.GetString(6),
                    ReleaseDate = reader.GetString(7),
                    Url = reader.GetString(8),
                };
            }
            return null;
        }

        public async Task<List<Anime>> GetAllAnimeAsync()
        {
            var query = "SELECT * FROM Anime";
            var animeList = new List<Anime>();

            using var connection = await DBManager.DBManager.GetConnectionAsync();
            using var command = new SQLiteCommand(query, connection);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                animeList.Add(new Anime
                {
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Rating = reader.GetString(2),
                    Description = reader.GetString(3),
                    Type = reader.GetString(4),
                    Genre = reader.GetString(5),
                    ImageURL = reader.GetString(6),
                    ReleaseDate = reader.GetString(7),
                    Url = reader.GetString(8),
                });
            }
            return animeList;
        }
    }
}