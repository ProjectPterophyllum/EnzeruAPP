namespace EnzeruAPP.Enzeru.Models;

public class UserAnimeList
{
    public int Id { get; set; }              // Уникальный идентификатор записи
    public int UserId { get; set; }           // Идентификатор пользователя (ссылка на таблицу User)
    public int AnimeId { get; set; }          // Идентификатор аниме (ссылка на таблицу Anime)
    public bool Status { get; set; }

    public User? User { get; set; }           // Ссылка на объект пользователя (если понадобится доступ к пользователю)
    public Anime? Anime { get; set; }

    public UserAnimeList() { }

    public UserAnimeList(int userId, int animeId, bool status)
    {
        UserId = userId;
        AnimeId = animeId;
        Status = status;
    }

    public override string ToString()
    {
        return $"UserAnimeList: ID = {Id}, UserID = {UserId}, AnimeID = {AnimeId}, Status = {Status}";
    }

}
