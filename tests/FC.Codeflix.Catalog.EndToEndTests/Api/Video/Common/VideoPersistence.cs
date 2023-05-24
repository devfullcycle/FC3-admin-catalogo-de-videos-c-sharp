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
}
