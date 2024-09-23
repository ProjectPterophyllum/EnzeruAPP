using System.Data.SQLite;
namespace EnzeruAPP.Enzeru.DBManager
{
    public class DBManager
    {
        private const string _dBFileName = "Enzeru.db";
        private const string _dBFolderPath = "./Enzeru.DBs";
        private const string _connectionString = $"Data Source={_dBFolderPath}/{_dBFileName};Version=3;";

        public static async Task<SQLiteConnection> GetConnectionAsync()
        {
            SQLiteConnection connection = new SQLiteConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
        public static async Task InitializeDatabaseAsync()
        {

            if (!Directory.Exists(_dBFolderPath))
            {
                Directory.CreateDirectory(_dBFolderPath);
            }


            if (!File.Exists($"{_dBFolderPath}/{_dBFileName}"))
            {
                await CreateDatabaseAsync();
            }

            await CreateTablesAsync();
        }


        private static async Task CreateDatabaseAsync()
        {
            try
            {
                using var connection = await GetConnectionAsync();
                Console.WriteLine("База данных успешно создана.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании базы данных: {ex.Message}");
            }
        }


        private static async Task CreateTablesAsync()
        {
            try
            {
                using var connection = await GetConnectionAsync();
                string createAnimeTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Anime (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT NOT NULL,
                            Rating TEXT NOT NULL,
                            Description TEXT NOT NULL,
                            Type TEXT NOT NULL,
                            Genre TEXT NOT NULL,
                            ImageURL TEXT NOT NULL,
                            ReleaseDate TEXT NOT NULL,
                            Url TEXT NOT NULL
                        );";

                string createUserTableQuery = @"
                        CREATE TABLE IF NOT EXISTS User (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT NOT NULL
                        );";

                string createUserAnimeListTableQuery = @"
                        CREATE TABLE IF NOT EXISTS UserAnimeList (
                            ID INTEGER PRIMARY KEY AUTOINCREMENT,
                            UserID INTEGER,
                            AnimeID INTEGER,
                            Status BOOLEAN,
                            FOREIGN KEY (UserID) REFERENCES User(ID),
                            FOREIGN KEY (AnimeID) REFERENCES Anime(ID)
                        );";

                using (var command = new SQLiteCommand(createAnimeTableQuery, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                using (var command = new SQLiteCommand(createUserTableQuery, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                using (var command = new SQLiteCommand(createUserAnimeListTableQuery, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                Console.WriteLine("Таблицы успешно созданы или уже существуют.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании таблиц: {ex.Message}");
            }
        }
    }
}

