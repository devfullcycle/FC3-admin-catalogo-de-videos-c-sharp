using FC.Codeflix.Catalog.Domain.Repository;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Domain.Enum;

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

    [Fact(DisplayName = nameof(InsertWithMediasAndImages))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task InsertWithMediasAndImages()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);

        await videoRepository.Insert(exampleVideo, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos
            .Include(x => x.Media)
            .Include(x => x.Trailer)
            .FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.Id.Should().Be(exampleVideo.Id);
        dbVideo.Thumb.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        dbVideo.ThumbHalf.Should().NotBeNull();
        dbVideo.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
        dbVideo.Media.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        dbVideo.Media.Status.Should().Be(exampleVideo.Media.Status);
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
        dbVideo.Trailer.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
        dbVideo.Trailer.Status.Should().Be(exampleVideo.Trailer.Status);
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
        var dbVideosCategories = assertsDbContext.VideosCategories
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosCategories.Should().HaveCount(categories.Count);
        dbVideosCategories.Select(relation => relation.CategoryId).ToList()
            .Should().BeEquivalentTo(
                categories.Select(category => category.Id));
        var dbVideosGenres = assertsDbContext.VideosGenres
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosGenres.Should().HaveCount(genres.Count);
        dbVideosGenres.Select(relation => relation.GenreId).ToList()
            .Should().BeEquivalentTo(
                genres.Select(genre => genre.Id));
        var dbVideosCastMembers = assertsDbContext.VideosCastMembers
            .Where(relation => relation.VideoId == exampleVideo.Id)
            .ToList();
        dbVideosCastMembers.Should().HaveCount(castMembers.Count);
        dbVideosCastMembers.Select(relation => relation.CastMemberId).ToList()
            .Should().BeEquivalentTo(
                castMembers.Select(castMember => castMember.Id));
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Update()
    {
        var dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var newValuesVideo = _fixture.GetExampleVideo();
        var dbContextAct = _fixture.CreateDbContext(true);
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContextAct);

        exampleVideo.Update(
            newValuesVideo.Title,
            newValuesVideo.Description,
            newValuesVideo.YearLaunched,
            newValuesVideo.Opened,
            newValuesVideo.Published,
            newValuesVideo.Duration,
            newValuesVideo.Rating);
        await videoRepository.Update(exampleVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

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

    [Fact(DisplayName = nameof(UpdateEntitiesAndValueObjects))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task UpdateEntitiesAndValueObjects()
    {
        var dbContextArrange = _fixture.CreateDbContext();
        var exampleVideo = _fixture.GetExampleVideo();
        await dbContextArrange.AddAsync(exampleVideo);
        await dbContextArrange.SaveChangesAsync();
        var updatedThumb = _fixture.GetValidImagePath();
        var updatedThumbHalf = _fixture.GetValidImagePath();
        var updatedBanner = _fixture.GetValidImagePath();
        var updatedMedia = _fixture.GetValidMediaPath();
        var updatedMediaEncoded = _fixture.GetValidMediaPath();
        var updatedTrailer = _fixture.GetValidMediaPath();
        var dbContextAct = _fixture.CreateDbContext(true);
        IVideoRepository videoRepository = new Repository.VideoRepository(dbContextAct);
        var savedVideo = dbContextAct.Videos
            .Single(video => video.Id == exampleVideo.Id);

        savedVideo.UpdateThumb(updatedThumb);
        savedVideo.UpdateThumbHalf(updatedThumbHalf);
        savedVideo.UpdateBanner(updatedBanner);
        savedVideo.UpdateTrailer(updatedTrailer);
        savedVideo.UpdateMedia(updatedMedia);
        savedVideo.UpdateAsEncoded(updatedMediaEncoded);
        await videoRepository.Update(savedVideo, CancellationToken.None);
        await dbContextAct.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
        dbVideo.Should().NotBeNull();
        dbVideo!.ThumbHalf.Should().NotBeNull();
        dbVideo.Thumb.Should().NotBeNull();
        dbVideo.Banner.Should().NotBeNull();
        dbVideo.Media.Should().NotBeNull();
        dbVideo.Trailer.Should().NotBeNull();
        dbVideo.Thumb!.Path.Should().Be(updatedThumb);
        dbVideo.ThumbHalf!.Path.Should().Be(updatedThumbHalf);
        dbVideo.Banner!.Path.Should().Be(updatedBanner);
        dbVideo.Media!.FilePath.Should().Be(updatedMedia);
        dbVideo.Media.EncodedPath.Should().Be(updatedMediaEncoded);
        dbVideo.Media.Status.Should().Be(MediaStatus.Completed);
        dbVideo.Trailer!.FilePath.Should().Be(updatedTrailer);
    }

    [Fact(DisplayName = nameof(UpdateWithRelations))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task UpdateWithRelations()
    {
        var id = Guid.Empty;
        var castMembers = _fixture.GetRandomCastMembersList();
        var categories = _fixture.GetRandomCategoriesList();
        var genres = _fixture.GetRandomGenresList();
        using(var dbContext = _fixture.CreateDbContext())
        {
            var exampleVideo = _fixture.GetExampleVideo();
            id = exampleVideo.Id;
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos
            .First(video => video.Id == id);
        IVideoRepository videoRepository = new Repository.VideoRepository(actDbContext);

        castMembers.ToList().ForEach(castMember
            => savedVideo.AddCastMember(castMember.Id));
        categories.ToList().ForEach(category
            => savedVideo.AddCategory(category.Id));
        genres.ToList().ForEach(genre
            => savedVideo.AddGenre(genre.Id));

        await videoRepository.Update(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(id);
        dbVideo.Should().NotBeNull();
        var dbVideosCategories = assertsDbContext.VideosCategories
            .Where(relation => relation.VideoId == id)
            .ToList();
        dbVideosCategories.Should().HaveCount(categories.Count);
        dbVideosCategories.Select(relation => relation.CategoryId).ToList()
            .Should().BeEquivalentTo(
                categories.Select(category => category.Id));
        var dbVideosGenres = assertsDbContext.VideosGenres
            .Where(relation => relation.VideoId == id)
            .ToList();
        dbVideosGenres.Should().HaveCount(genres.Count);
        dbVideosGenres.Select(relation => relation.GenreId).ToList()
            .Should().BeEquivalentTo(
                genres.Select(genre => genre.Id));
        var dbVideosCastMembers = assertsDbContext.VideosCastMembers
            .Where(relation => relation.VideoId == id)
            .ToList();
        dbVideosCastMembers.Should().HaveCount(castMembers.Count);
        dbVideosCastMembers.Select(relation => relation.CastMemberId).ToList()
            .Should().BeEquivalentTo(
                castMembers.Select(castMember => castMember.Id));
    }
}
