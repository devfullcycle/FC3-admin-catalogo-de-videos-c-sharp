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
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;

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

    [Fact(DisplayName = nameof(Delete))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Delete()
    {
        var id = Guid.Empty;
        using(var dbContext = _fixture.CreateDbContext())
        {
            var exampleVideo = _fixture.GetExampleVideo();
            id = exampleVideo.Id;
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos.First(video => video.Id == id);
        IVideoRepository videoRepository = new Repository.VideoRepository(actDbContext);

        await videoRepository.Delete(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(id);
        dbVideo.Should().BeNull();
    }

    [Fact(DisplayName = nameof(DeleteWithAllPropertiesAndRelations))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task DeleteWithAllPropertiesAndRelations()
    {
        var id = Guid.Empty;
        using(var dbContext = _fixture.CreateDbContext())
        {
            var exampleVideo = _fixture.GetValidVideoWithAllProperties();
            id = exampleVideo.Id;
            var castMembers = _fixture.GetRandomCastMembersList();
            var categories = _fixture.GetRandomCategoriesList();
            var genres = _fixture.GetRandomGenresList();
            castMembers.ToList().ForEach(castMember
                => dbContext.VideosCastMembers.Add(new(castMember.Id, id)));
            categories.ToList().ForEach(category
                => dbContext.VideosCategories.Add(new(category.Id, id)));
            genres.ToList().ForEach(genre
                => dbContext.VideosGenres.Add(new(genre.Id, id)));
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var savedVideo = actDbContext.Videos.First(video => video.Id == id);
        IVideoRepository videoRepository = new Repository.VideoRepository(actDbContext);

        await videoRepository.Delete(savedVideo, CancellationToken.None);
        await actDbContext.SaveChangesAsync(CancellationToken.None);

        var assertsDbContext = _fixture.CreateDbContext(true);
        var dbVideo = await assertsDbContext.Videos.FindAsync(id);
        dbVideo.Should().BeNull();
        assertsDbContext.VideosCategories
            .Where(relation => relation.VideoId == id)
            .ToList().Count().Should().Be(0);
        assertsDbContext.VideosGenres
            .Where(relation => relation.VideoId == id)
            .ToList().Count().Should().Be(0);
        assertsDbContext.VideosCastMembers
            .Where(relation => relation.VideoId == id)
            .ToList().Count().Should().Be(0);
        assertsDbContext.Set<Media>().Count().Should().Be(0);
    }

    [Fact(DisplayName = nameof(Get))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Get()
    {
        var exampleVideo = _fixture.GetExampleVideo();
        using(var dbContext = _fixture.CreateDbContext())
        {
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }
        IVideoRepository videoRepository = 
            new Repository.VideoRepository(_fixture.CreateDbContext(true));

        var video = await videoRepository.Get(exampleVideo.Id, CancellationToken.None);

        video!.Id.Should().Be(exampleVideo.Id);
        video.Title.Should().Be(exampleVideo.Title);
        video.Description.Should().Be(exampleVideo.Description);
        video.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        video.Opened.Should().Be(exampleVideo.Opened);
        video.Published.Should().Be(exampleVideo.Published);
        video.Duration.Should().Be(exampleVideo.Duration);
        video.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        video.Rating.Should().Be(exampleVideo.Rating);
        video.Should().NotBeNull();
        video!.ThumbHalf.Should().BeNull();
        video.Thumb.Should().BeNull();
        video.Banner.Should().BeNull();
        video.Media.Should().BeNull();
        video.Trailer.Should().BeNull();
    }


    [Fact(DisplayName = nameof(GetWithAllProperties))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task GetWithAllProperties()
    {
        var id = Guid.Empty;
        var exampleVideo = _fixture.GetValidVideoWithAllProperties();
        using(var dbContext = _fixture.CreateDbContext())
        {
            id = exampleVideo.Id;
            var castMembers = _fixture.GetRandomCastMembersList();
            var categories = _fixture.GetRandomCategoriesList();
            var genres = _fixture.GetRandomGenresList();
            castMembers.ToList().ForEach(castMember => {
                exampleVideo.AddCastMember(castMember.Id);
                dbContext.VideosCastMembers.Add(new(castMember.Id, id));
            });
            categories.ToList().ForEach(category => {
                exampleVideo.AddCategory(category.Id);
                dbContext.VideosCategories.Add(new(category.Id, id));
            });
            genres.ToList().ForEach(genre =>
            {
                exampleVideo.AddGenre(genre.Id);
                dbContext.VideosGenres.Add(new(genre.Id, id));
            });
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            await dbContext.Categories.AddRangeAsync(categories);
            await dbContext.Genres.AddRangeAsync(genres);
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.Videos.AddAsync(exampleVideo);
            await dbContext.SaveChangesAsync();
        }
        IVideoRepository videoRepository =
            new Repository.VideoRepository(_fixture.CreateDbContext(true));

        var video = await videoRepository.Get(id, CancellationToken.None);

        video.Should().NotBeNull();
        video!.Id.Should().Be(exampleVideo.Id);
        video.Title.Should().Be(exampleVideo.Title);
        video.Description.Should().Be(exampleVideo.Description);
        video.YearLaunched.Should().Be(exampleVideo.YearLaunched);
        video.Opened.Should().Be(exampleVideo.Opened);
        video.Published.Should().Be(exampleVideo.Published);
        video.Duration.Should().Be(exampleVideo.Duration);
        video.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
        video.Rating.Should().Be(exampleVideo.Rating);
        video.ThumbHalf.Should().NotBeNull();
        video.Thumb.Should().NotBeNull();
        video.Banner.Should().NotBeNull();
        video.Media.Should().NotBeNull();
        video.Trailer.Should().NotBeNull();
        video.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
        video.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
        video.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
        video.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
        video.Media.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
        video.Media.Status.Should().Be(exampleVideo.Media.Status);
        video.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
        video.Trailer.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
        video.Trailer.Status.Should().Be(exampleVideo.Trailer.Status);
        video.Genres.Should().BeEquivalentTo(exampleVideo.Genres);
        video.Categories.Should().BeEquivalentTo(exampleVideo.Categories);
        video.CastMembers.Should().BeEquivalentTo(exampleVideo.CastMembers);
    }

    [Fact(DisplayName = nameof(GetThrowIfNotFind))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task GetThrowIfNotFind()
    {
        var id = Guid.NewGuid();
        IVideoRepository videoRepository =
            new Repository.VideoRepository(_fixture.CreateDbContext());

        var action = () => videoRepository.Get(id, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Video '{id}' not found.");
    }
    
    [Fact(DisplayName = nameof(Search))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task Search()
    {
        var exampleVideosList = _fixture.GetExampleVideosList();
        using(var arrangeDbContext = _fixture.CreateDbContext())
        {
            await arrangeDbContext.Videos.AddRangeAsync(exampleVideosList);
            await arrangeDbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(exampleVideosList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(exampleVideosList.Count);
        result.Items.ToList().ForEach(resultItem => {
            var exampleVideo = exampleVideosList
                .FirstOrDefault(x => x.Id == resultItem.Id);
            exampleVideosList.Should().NotBeNull();
            resultItem!.Id.Should().Be(exampleVideo!.Id);
            resultItem.Title.Should().Be(exampleVideo.Title);
            resultItem.Description.Should().Be(exampleVideo.Description);
            resultItem.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            resultItem.Opened.Should().Be(exampleVideo.Opened);
            resultItem.Published.Should().Be(exampleVideo.Published);
            resultItem.Duration.Should().Be(exampleVideo.Duration);
            resultItem.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));
            resultItem.Rating.Should().Be(exampleVideo.Rating);
            resultItem.Thumb.Should().BeNull();
            resultItem.ThumbHalf.Should().BeNull();
            resultItem.Banner.Should().BeNull();
            resultItem.Media.Should().BeNull();
            resultItem.Trailer.Should().BeNull();
            resultItem.Genres.Should().BeEmpty();
            resultItem.Categories.Should().BeEmpty();
            resultItem.CastMembers.Should().BeEmpty();

        });
    }
    
    [Fact(DisplayName = nameof(SearchReturnsEmptyWhenEmpty))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    public async Task SearchReturnsEmptyWhenEmpty()
    {
        var actDbContext = _fixture.CreateDbContext();
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(1, 20, "", "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(0);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(0);
    }
    
    [Theory(DisplayName = nameof(SearchPagination))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchPagination(
        int quantityToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        var exampleVideosList = _fixture.GetExampleVideosList(quantityToGenerate);
        using(var arrangeDbContext = _fixture.CreateDbContext())
        {
            await arrangeDbContext.Videos.AddRangeAsync(exampleVideosList);
            await arrangeDbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, "", "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(exampleVideosList.Count);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(expectedQuantityItems);
        result.Items.ToList().ForEach(resultItem => {
            var exampleVideo = exampleVideosList
                .FirstOrDefault(x => x.Id == resultItem.Id);
            exampleVideosList.Should().NotBeNull();
            resultItem!.Id.Should().Be(exampleVideo!.Id);
            resultItem.Title.Should().Be(exampleVideo.Title);
            resultItem.Description.Should().Be(exampleVideo.Description);
        });
    }
    
    [Theory(DisplayName = nameof(SearchByTitle))]
    [Trait("Integration/Infra.Data", "Video Repository - Repositories")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByTitle(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        var exampleVideosList = _fixture.GetExampleVideosListByTitles(
            new() {
                "Action",
                "Horror",
                "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Sci-fi IA",
                "Sci-fi Space",
                "Sci-fi Robots",
                "Sci-fi Future" });
        using(var arrangeDbContext = _fixture.CreateDbContext())
        {
            await arrangeDbContext.Videos.AddRangeAsync(exampleVideosList);
            await arrangeDbContext.SaveChangesAsync();
        }
        var actDbContext = _fixture.CreateDbContext(true);
        var videoRepository = new Repository.VideoRepository(actDbContext);
        var searchInput = new SearchInput(page, perPage, search, "", default);

        var result = await videoRepository.Search(searchInput, CancellationToken.None);

        result.CurrentPage.Should().Be(searchInput.Page);
        result.PerPage.Should().Be(searchInput.PerPage);
        result.Total.Should().Be(expectedQuantityTotalItems);
        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(expectedQuantityItemsReturned);
        result.Items.ToList().ForEach(resultItem => {
            var exampleVideo = exampleVideosList
                .FirstOrDefault(x => x.Id == resultItem.Id);
            exampleVideosList.Should().NotBeNull();
            resultItem!.Id.Should().Be(exampleVideo!.Id);
            resultItem.Title.Should().Be(exampleVideo.Title);
            resultItem.Description.Should().Be(exampleVideo.Description);
        });
    }
}
