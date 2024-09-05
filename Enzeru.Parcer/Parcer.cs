using System.Threading.Tasks;
using HtmlAgilityPack;
using EnzeruAPP.Enzeru.Models;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EnzeruAPP.Enzeru.Parcer;

public class AnimeRatingParser
{
    private readonly HttpClient _httpClient;

    public AnimeRatingParser()
    {
        _httpClient = new HttpClient();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<List<Anime>> GetAnimeListAsync(string url)
    {
        var animeList = new List<Anime>();

        try
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(contentStream, Encoding.GetEncoding("windows-1251"));
            var pageContents = await reader.ReadToEndAsync();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageContents);

            // Извлечение всех ссылок на аниме с классом 'review'
            var titleNodes = htmlDocument.DocumentNode.SelectNodes("//a[contains(@class, 'review')]");

            // Извлечение всех рейтингов с классом 'review'
            var ratingNodes = htmlDocument.DocumentNode.SelectNodes("//td[contains(@class, 'review')]");

            if (titleNodes == null || ratingNodes == null)
            {
                Console.WriteLine("Не удалось найти элементы с классом 'review'.");
                return animeList;
            }

            // Регулярное выражение для проверки формата рейтинга
            var ratingPattern = new Regex(@"^\d+(\.\d+)?$");

            // Проверяем, что количество заголовков и рейтингов совпадает
            int count = Math.Min(titleNodes.Count, ratingNodes.Count);

            for (int i = 0; i < count; i++)
            {
                var titleNode = titleNodes[i];
                var ratingNode = ratingNodes[i];

                // Фильтрация ссылок, содержащих "/votes_history"
                var href = titleNode.GetAttributeValue("href", string.Empty);
                if (href.Contains("/votes_history") || href.Contains("rating_top.php"))
                {
                    continue;
                }

                var titleText = titleNode.InnerText.Trim();
                var cleanedTitle = HttpUtility.HtmlDecode(titleText);

                // Получение рейтинга и проверка формата
                var ratingText = ratingNode.InnerText.Trim();
                if (!ratingPattern.IsMatch(ratingText))
                {
                    continue;
                }

                var anime = new Anime
                {
                    Title = cleanedTitle,
                    Url = href,
                    Rating = ratingText
                };

                animeList.Add(anime);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при парсинге страницы: {ex.Message}");
        }

        return animeList;
    }
}
//2-5200