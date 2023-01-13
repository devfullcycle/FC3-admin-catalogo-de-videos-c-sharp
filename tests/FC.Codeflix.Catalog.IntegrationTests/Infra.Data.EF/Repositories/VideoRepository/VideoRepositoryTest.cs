using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using System.Linq;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository;

[Collection(nameof(VideoRepositoryTestFixture))]
public class VideoRepositoryTest
{
    private readonly VideoRepositoryTestFixture _fixture;

    public VideoRepositoryTest(VideoRepositoryTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Insert))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Insert()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Title.Should().Be(exampleVideo.Title);
        dbVideo.Description.Should().Be(exampleVideo.Description);
        dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        dbVideo.Opened.Should().Be(exampleVideo.Opened);
        dbVideo.Published.Should().Be(exampleVideo.Published);
        dbVideo.Duration.Should().Be(exampleVideo.Duration);
        dbVideo.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        dbVideo.Rating.Should().Be(exampleVideo.Rating);

        dbVideo.Thumb.Should().BeNull();
        dbVideo.ThumbHalf.Should().BeNull();
        dbVideo.Banner.Should().BeNull();
        dbVideo.Media.Should().BeNull();
        dbVideo.Trailer.Should().BeNull();

        dbVideo.Genres.Should().BeEmpty();
        dbVideo.Categories.Should().BeEmpty();
        dbVideo.CastMembers.Should().BeEmpty();
    }

    [Fact(DisplayName = nameof(InsertWithRelations))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task InsertWithRelations()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        var castMembers = _fixture.GetRandomCastMembersList();
        castMembers.ToList().ForEach(castMember 
            => exampleVideo.AddCastMember(castMember.Id));
        await dbContext.CastMembers.AddRangeAsync(castMembers);
        var categories = _fixture.GetRandomCategoriesList();
        categories.ToList().ForEach(category
            => exampleVideo.AddCategory(category.Id));
        await dbContext.Categories.AddRangeAsync(categories);
        var genres = _fixture.GetRandomGenresList();
        genres.ToList().ForEach(genre
            => exampleVideo.AddGenre(genre.Id));
        await dbContext.Genres.AddRangeAsync(genres);
        await dbContext.SaveChangesAsync();
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);
        
        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Genres.Should().HaveCount(genres.Count());
        dbVideo.Genres.Should().BeEquivalentTo(
            genres.Select(genre => genre.Id));
        dbVideo.Categories.Should().HaveCount(categories.Count());
        dbVideo.Categories.Should().BeEquivalentTo(
            categories.Select(category => category.Id));
        dbVideo.CastMembers.Should().HaveCount(castMembers.Count());
        dbVideo.CastMembers.Should().BeEquivalentTo(
            castMembers.Select(castMember => castMember.Id));
    }
}
