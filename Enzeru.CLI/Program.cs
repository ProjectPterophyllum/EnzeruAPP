using System;
using EnzeruAPP.Enzeru.DBManager;
using EnzeruAPP.Enzeru.Parcer;
using EnzeruAPP.Enzeru.Models;
using EnzeruAPP.Enzeru.Repository;
using EnzeruAPP.Enzeru.Repository.Classes;
using EnzeruAPP.Enzeru.Repository.Interface;
using System.Diagnostics;

await DBManager.InitializeDatabaseAsync();

Stopwatch stopwatch = new Stopwatch();
var parser = new AnimeRatingParcer();
string url = "http://www.world-art.ru/animation/rating_top.php";
stopwatch.Start();
var animeList = await parser.GetAnimeListAsync(url);
stopwatch.Stop();
TimeSpan ts = stopwatch.Elapsed;
string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
ts.Hours, ts.Minutes, ts.Seconds,
ts.Milliseconds / 10);
Console.WriteLine("RunTime: " + elapsedTime);
foreach (var anime in animeList)
{
    var animeRepository = new AnimeRepository();
    await animeRepository.InsertAnimeAsync(anime);
}
