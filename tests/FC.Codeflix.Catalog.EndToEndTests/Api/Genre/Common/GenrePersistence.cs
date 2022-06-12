using FC.Codeflix.Catalog.Infra.Data.EF;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

using System.Threading.Tasks;
using System.Collections.Generic;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenrePersistence
{
    private readonly CodeflixCatalogDbContext _context;

    public GenrePersistence(CodeflixCatalogDbContext context)
        => _context = context;

    public async Task InsertList(List<DomainEntity.Genre> genres)
    {
        await _context.AddRangeAsync(genres);
        await _context.SaveChangesAsync();
    }

    public async Task InsertGenresCategoriesRelationsList(List<GenresCategories> relations)
    {
        await _context.AddRangeAsync(relations);
        await _context.SaveChangesAsync();
    }

    public async Task<DomainEntity.Genre?> GetById(Guid id)
        => await _context.Genres.AsNoTracking()
            .FirstOrDefaultAsync(genre => genre.Id == id);

    internal async Task<List<GenresCategories>> GetGenresCategoriesRelationsByGenreId(Guid id)
        => await _context.GenresCategories.AsNoTracking()
            .Where(relation => relation.GenreId == id)
            .ToListAsync();
}
