using FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

internal class VideoConfiguration
    : IEntityTypeConfiguration<Video> 
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.HasKey(video => video.Id);

        builder.Navigation(x => x.Media).AutoInclude();
        builder.Navigation(x => x.Trailer).AutoInclude();
        
        builder.Property(video => video.Id)
            .ValueGeneratedNever();
        builder.Property(video => video.Title)
            .HasMaxLength(255);
        builder.Property(video => video.Description)
            .HasMaxLength(4_000);
        
        builder.OwnsOne(video => video.Thumb, thumb =>
            thumb.Property(image => image.Path).HasColumnName("ThumbPath")
        );
        builder.OwnsOne(video => video.ThumbHalf, thumbHalf =>
            thumbHalf.Property(image => image.Path).HasColumnName("ThumbHalfPath")
        );
        builder.OwnsOne(video => video.Banner, banner =>
            banner.Property(image => image.Path).HasColumnName("bannerPath")
        );

        builder.HasOne(video => video.Media).WithOne().HasForeignKey<Media>();
        builder.HasOne(video => video.Trailer).WithOne().HasForeignKey<Media>();
        builder.Ignore(video => video.Events);
    }
}
