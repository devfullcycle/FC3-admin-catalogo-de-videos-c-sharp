using FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models;

public class VideosCategories
{
    public VideosCategories(
        Guid categoryId,
        Guid videoId
    )
    {
        CategoryId = categoryId;
        VideoId = videoId;
    }

    public Guid CategoryId { get; set; }
    public Guid VideoId { get; set; }
    public Category? Category { get; set; }
    public Video? Video { get; set; }
}
