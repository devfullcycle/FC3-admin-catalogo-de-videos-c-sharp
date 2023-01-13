using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF.Configurations;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF;
public class CodeflixCatalogDbContext
    : DbContext
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Video> Videos => Set<Video>();

    public DbSet<GenresCategories> GenresCategories 
        => Set<GenresCategories>();
    public DbSet<VideosCategories> VideosCategories 
        => Set<VideosCategories>();
    public DbSet<VideosGenres> VideosGenres
        => Set<VideosGenres>();
    public DbSet<VideosCastMembers> VideosCastMembers
            => Set<VideosCastMembers>();

    public CodeflixCatalogDbContext(
        DbContextOptions<CodeflixCatalogDbContext> options
    ) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new GenreConfiguration());
        builder.ApplyConfiguration(new VideoConfiguration());

        builder.ApplyConfiguration(new GenresCategoriesConfiguration());
        builder.ApplyConfiguration(new VideosCategoriesConfiguration());
        builder.ApplyConfiguration(new VideosGenresConfiguration());
        builder.ApplyConfiguration(new VideosCastMembersConfiguration());
    }
}
