using AngleSharp;
using EnzeruAPP.Enzeru.Models;
using System.Text;
using System.Web;
using AngleSharp.Dom;
using AngleSharp.Io.Network;

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
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string html = await response.Content.ReadAsStringAsync();
            return await _context.OpenAsync(request => request.Header("Content-Type", "text/html; charset=utf-8").Content(html)) ?? throw new Exception("Не удалось загрузить страницу.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке страницы: {ex.Message}");
            return null;
        }
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
                GetAdditionalInfoFromAnimeAsync(anime).Wait();
                animeList.Add(anime);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при парсинге страницы: {ex.Message}");
        }

        return animeList;
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


            var document = await GetAnimePageAsync(anime.Url);
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
                if (td.TextContent.Trim() == "Сезон")
                {
                    var seasonDateNode = document.QuerySelector("a.review[href*='list.php?public_season']");
                    anime.ReleaseDate = seasonDateNode.TextContent.Trim();
                }
                else if (td.TextContent.Trim() == "Премьера")
                {
                    var premiereDateNodes = document.QuerySelectorAll("a[href *= 'list.php?public_year']");
                    anime.ReleaseDate = string.Join(".", premiereDateNodes.Select(node => node.TextContent.Trim()));
                }

            }
            var imageNode = document.QuerySelector("img[border='1']");
            anime.ImageURL = imageNode?.GetAttribute("src") ?? "Не указано";

            var descriptionNode = document.QuerySelector("p.review");
            anime.Description = descriptionNode?.TextContent.Trim() ?? "Не указано";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении дополнительной информации: {ex.Message}");
        }
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}
//2-5200