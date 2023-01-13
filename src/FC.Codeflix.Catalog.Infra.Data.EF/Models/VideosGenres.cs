using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;

public class VideosGenres
{
    public VideosGenres(
        Guid genreId,
        Guid videoId
    )
    {
        GenreId = genreId;
        VideoId = videoId;
    }

    public Guid GenreId { get; set; }
    public Guid VideoId { get; set; }
    public Genre? Genre { get; set; }
    public Video? Video { get; set; }
}
