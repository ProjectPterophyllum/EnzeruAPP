using AngleSharp;
using EnzeruAPP.Enzeru.Models;
using System.Text;
using System.Web;
using AngleSharp.Dom;
using AngleSharp.Io.Network;
using ImageMagick;

namespace EnzeruAPP.Enzeru.Parcer;

public class AnimeRatingParcer
{
    private readonly HttpClient _httpClient;
    private readonly IBrowsingContext _context;

    public AnimeRatingParcer(HttpClient httpClient, IBrowsingContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

    public AnimeRatingParcer()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _httpClient = new HttpClient();
        _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
        var config = Configuration.Default;
        var req = new HttpClientRequester(_httpClient);
        config.WithRequester(req);
        config.WithCss();
        config.WithDefaultLoader();
        _context = BrowsingContext.New(config);
    }
    public async Task<IDocument?> GetAnimePageAsync(string url)
    {
        var delayMs = 10000;
        for (int attempts = 0; attempts < 5; attempts++)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                {
                    Console.WriteLine($"Статус код: {response.StatusCode}, повторная попытка через {delayMs} миллисекунд");
                    await Task.Delay(delayMs);
                    delayMs *= 2;
                    continue;
                }
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();
                return await _context.OpenAsync(request => request.Header("Content-Type", "text/html; charset=utf-8").Content(html)) ?? throw new Exception("Не удалось загрузить страницу.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке страницы: {ex.Message}");
            }
        }
        return null;
    }
    public async Task<List<Anime>> GetAnimeListAsync(string url)
    {
        var animeList = new List<Anime>();

        try
        {
            var document = await GetAnimePageAsync(url);

            var titleNodes = document?.QuerySelectorAll("tr[height='20']");

            var ratingNodes = document?.QuerySelectorAll("tr[height='20']");

            int count = Math.Min(titleNodes.Length, ratingNodes.Length);

            for (int i = 0; i < count; i++)
            {
                var anime = new Anime();
                var titleNode = titleNodes[i];
                var ratingNode = ratingNodes[i];

                var titleText = titleNode.QuerySelector("td:nth-child(2) a.review").TextContent.Trim();

                var href = titleNode.QuerySelector("td:nth-child(2) a.review").GetAttribute("href");

                var ratingText = ratingNode.QuerySelector("td:last-child").TextContent.Trim();

                anime.Title = HttpUtility.HtmlDecode(titleText);
                anime.Rating = ratingText;
                anime.Url = href;
                animeList.Add(anime);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при парсинге страницы: {ex.Message}");
        }

        return animeList;
    }

    public async Task<List<Anime>> GetAnimeListAsyncWithDelay(string url, int delayMs)
    {
        // Добавляем задержку перед выполнением запроса
        await Task.Delay(delayMs);

        // Выполняем парсинг
        return await GetAnimeListAsync(url);
    }

    public async Task GetAdditionalInfoFromAnimeAsync(Anime anime)
    {
        try
        {
            if (string.IsNullOrEmpty(anime.Url))
            {
                Console.WriteLine("URL аниме отсутствует.");
                return;
            }
            try
            {
                var document = await GetAnimePageAsync(anime.Url);
                if (document == null)
                {
                    throw new Exception("Получена пустая страница.");
                }
                var selector = "td.review";
                var typeNode = document.QuerySelectorAll(selector);
                foreach (var td in typeNode)
                {
                    if (td.TextContent.Trim() == "Жанр")
                    {
                        var genreNodes = document.QuerySelectorAll("a.review[href*='list.php?public_genre']");
                        if (genreNodes.Length > 0)
                        {
                            var genres = genreNodes.Select(genreNode => genreNode.TextContent.Trim()).ToList();
                            anime.Genre = string.Join(", ", genres);
                        }
                    }
                    if (td.TextContent.Trim() == "Тип")
                    {
                        var targetElement = td.NextElementSibling?.NextElementSibling;

                        if (targetElement != null)
                        {
                            anime.Type = targetElement.TextContent.Trim();
                        }
                    }
                    if (td.TextContent.Trim() == "Выпуск")
                    {
                        var releaseDateNodes = document.QuerySelectorAll("a[href *= 'list.php?public_year']");
                        anime.ReleaseDate = string.Join(".", releaseDateNodes.Select(node => node.TextContent.Trim()));
                        break;
                    }
                    else if (td.TextContent.Trim() == "Премьера")
                    {
                        var premiereDateNodes = document.QuerySelectorAll("a[href *= 'list.php?public_year']");
                        anime.ReleaseDate = string.Join(".", premiereDateNodes.Select(node => node.TextContent.Trim()));
                        break;
                    }
                    else if (td.TextContent.Trim() == "Сезон")
                    {
                        var seasonDateNode = document.QuerySelector("a.review[href*='list.php?public_season']");
                        anime.ReleaseDate = seasonDateNode.TextContent.Trim();
                        break;
                    }
                }
                anime.Type ??= "Не указано";

                anime.Genre ??= "Не указано";

                anime.ReleaseDate ??= "Не указано";

                var imageNode = document.QuerySelector("img[border='1']");
                anime.ImageURL = imageNode?.GetAttribute("src") ?? "Не указано";

                var descriptionNode = document.QuerySelector("p.review");
                anime.Description = descriptionNode?.TextContent.Trim() ?? "Не указано";
                Console.WriteLine("Дополнительная информация успешно получена. {0} {1} {2} {3} {4} ", anime.Title, anime.Type, anime.Genre, anime.ReleaseDate, anime.ImageURL);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении страницы: {ex.Message}");
                anime.Description = "Не удалось получить описание аниме.";
                anime.Type = "Неизвестно";
                anime.Genre = "Неизвестно";
                anime.ReleaseDate = "Неизвестно";
                anime.ImageURL = "Неизвестно";

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении дополнительной информации: {ex.Message}");
        }
    }
    public async Task GetAdditionalInfoFromAnimeAsyncWithDelay(Anime anime, int delayMs)
    {
        await Task.Delay(delayMs);

        await GetAdditionalInfoFromAnimeAsync(anime);
    }

    public async Task GetAnimePoster(Anime anime)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "Images", anime.ID.ToString());
        var fileName = anime.Title;
        fileName = fileName.Replace("/", "-").Replace("\\", "-");
        try
        {
            if (string.IsNullOrEmpty(anime.ImageURL))
            {
                throw new Exception("URL картинки отсутствует.");
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(Path.Combine(path, $"{fileName}.jpg")))
            {
                HttpResponseMessage response = await _httpClient.GetAsync(anime.ImageURL);
                response.EnsureSuccessStatusCode();
                using (MagickImage image = new MagickImage(response.Content.ReadAsStreamAsync().Result))
                {
                    await image.WriteAsync(Path.Combine(path, $"{fileName}.jpg"));
                    Console.WriteLine("Постер успешно получен. {0} ", anime.Title);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Невозможно получить постер. Ошибка при получении постера: {ex.Message}");
            if (!File.Exists(Path.Combine(path, $"{fileName}.jpg")))
            {
                using (MagickImage image = new MagickImage(MagickColors.Aquamarine, 800, 600))
                {
                    await image.WriteAsync(Path.Combine(path, $"{fileName}.jpg"));
                    Console.WriteLine("Создана заглушка для постера. {0} ", anime.Title);
                }
            }
            return;
        }
    }
}