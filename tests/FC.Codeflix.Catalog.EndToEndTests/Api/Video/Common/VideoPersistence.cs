using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Video.Common;
public class VideoPersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public VideoPersistence(CodeflixCatalogDbContext context)
        => _context = context;

    public async Task<DomainEntity.Video?> GetById(Guid id)
        => await _context.Videos.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<VideosCastMembers>> GetVideosCastMembers(Guid videoId)
        => await _context.VideosCastMembers.AsNoTracking()
            .Where(relation => relation.VideoId == videoId)
            .ToListAsync();

    public async Task<List<VideosGenres>> GetVideosGenres(Guid videoId)
        => await _context.VideosGenres.AsNoTracking()
            .Where(relation => relation.VideoId == videoId)
            .ToListAsync();

    public async Task<List<VideosCategories>> GetVideosCategories(Guid videoId)
        => await _context.VideosCategories.AsNoTracking()
            .Where(relation => relation.VideoId == videoId)
            .ToListAsync();

    public async Task<int> GetMediaCount()
        => await _context.Set<Media>().CountAsync();

    public async Task InsertList(List<DomainEntity.Video> videos)
    {
        await _context.Videos.AddRangeAsync(videos);
        foreach (var video in videos)
        {
            var videosCategories = video.Categories?
                .Select(categoryId => new VideosCategories(categoryId, video.Id));
            if (videosCategories != null && videosCategories.Any())
            {
                await _context.VideosCategories.AddRangeAsync(videosCategories);
            }

            var videosGenres = video.Genres?
                .Select(genreId => new VideosGenres(genreId, video.Id));
            if (videosGenres != null && videosGenres.Any())
            {
                await _context.VideosGenres.AddRangeAsync(videosGenres);
            }

            var videosCastMembers = video.CastMembers?
                .Select(castMemberId => new VideosCastMembers(castMemberId, video.Id));
            if (videosCastMembers != null && videosCastMembers.Any())
            {
                await _context.VideosCastMembers.AddRangeAsync(videosCastMembers);
            }
        }
        await _context.SaveChangesAsync();
    }
}
