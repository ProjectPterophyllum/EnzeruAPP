using System;
using EnzeruAPP.Enzeru.DBManager;
using EnzeruAPP.Enzeru.Parcer;


namespace Enzeru.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var parser = new AnimeRatingParcer();
            string url = "http://www.world-art.ru/animation/rating_top.php";

            var animeList = await parser.GetAnimeListAsync(url);

            foreach (var anime in animeList)
            {
                Console.WriteLine($"Название: {anime.Title}");
                Console.WriteLine($"Рейтинг: {anime.Rating}");
                Console.WriteLine($"Ссылка: {anime.Url}");
                //Console.WriteLine($"Описание: {anime.Description}");
                Console.WriteLine($"Тип: {anime.Type}");
                Console.WriteLine($"Жанр: {anime.Genre}");
                Console.WriteLine($"Дата выхода: {anime.ReleaseDate}");
                Console.WriteLine($"Изображение: {anime.ImageURL}");
                Console.WriteLine(new string('-', 50));
            }
        }
    }
}