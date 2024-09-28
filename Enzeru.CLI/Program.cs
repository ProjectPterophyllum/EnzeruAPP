using EnzeruAPP.Enzeru.DBManager;
using EnzeruAPP.Enzeru.Parcer;
using EnzeruAPP.Enzeru.Models;
using EnzeruAPP.Enzeru.Repository.Classes;

// const int maxBegin = 5100;
// const int pageSize = 100;

var parcer = new AnimeRatingParcer();

// var DBManager = new DBManager();
var AnimeRepository = new AnimeRepository();

var animeList = new List<Anime>();

// for (int begin = 0; begin <= maxBegin; begin += pageSize)
// {
//     int end = begin + pageSize;
//     string url = $"http://www.world-art.ru/animation/rating_top.php?limit_1={begin}&limit_2={end}";
//     var temp = await parcer.GetAnimeListAsync(url);
//     animeList.AddRange(temp);
//     Console.WriteLine($"Добавлено {animeList.Count} аниме.");
// }

// foreach (var anime in animeList)
// {
//     await parcer.GetAdditionalInfoFromAnimeAsync(anime);
// }

// await DBManager.InitializeDatabaseAsync();

// foreach (var anime in animeList)
// {
//     await AnimeRepository.InsertAnimeAsync(anime);
// }

animeList = await AnimeRepository.GetAllAnimeAsync();

foreach (var anime in animeList)
{
    await parcer.GetAnimePoster(anime);
}