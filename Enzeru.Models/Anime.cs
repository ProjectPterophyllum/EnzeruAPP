namespace EnzeruAPP.Enzeru.Models;

public class Anime
{
    public int ID { get; set; }
    public string? Title { get; set; }
    public string? Rating { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public string? Genre { get; set; }
    public string? ImageURL { get; set; }
    public string? ReleaseDate { get; set; }
    public string? Url { get; set; }

    public Anime() { }

    public Anime(string title, string rating, string description, string type, string genre, string imageURL, string releaseDate, string url)
    {
        Title = title;
        Rating = rating;
        Description = description;
        Type = type;
        Genre = genre;
        ImageURL = imageURL;
        ReleaseDate = releaseDate;
        Url = url;
    }

    public override string ToString()
    {
        return $"Anime: ID = {ID}, Title = {Title}, Type = {Type}, Genre = {Genre}, ReleaseDate = {ReleaseDate}";
    }
}
