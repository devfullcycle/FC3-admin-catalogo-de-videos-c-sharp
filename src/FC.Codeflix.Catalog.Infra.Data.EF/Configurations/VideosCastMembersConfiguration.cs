using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Configurations;

internal class VideosCastMembersConfiguration
    : IEntityTypeConfiguration<VideosCastMembers>
{
    public void Configure(EntityTypeBuilder<VideosCastMembers> builder)
        => builder.HasKey(relation => new
        {
            relation.CastMemberId,
            relation.VideoId
        });
}
