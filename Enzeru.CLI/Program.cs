using System;
using EnzeruAPP.Enzeru.DBManager;
using EnzeruAPP.Enzeru.Parcer;


namespace Enzeru.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var parser = new AnimeRatingParser();
            string url = "http://www.world-art.ru/animation/rating_top.php?limit_1=0&&limit_2=100";

            var animeList = await parser.GetAnimeListAsync(url);

            foreach (var anime in animeList)
            {
                Console.WriteLine($"Название: {anime.Title}");
                Console.WriteLine($"Рейтинг: {anime.Rating}");
                Console.WriteLine($"Ссылка: {anime.Url}");
                Console.WriteLine(new string('-', 50));
            }
        }
    }
}