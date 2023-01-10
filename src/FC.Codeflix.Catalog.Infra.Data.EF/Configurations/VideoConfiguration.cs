using FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

internal class VideoConfiguration
    : IEntityTypeConfiguration<Video> 
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(video => video.Id);
        builder.Property(video => video.Title)
            .HasMaxLength(255);
        builder.Property(video => video.Description)
            .HasMaxLength(4_000);
    }
}
