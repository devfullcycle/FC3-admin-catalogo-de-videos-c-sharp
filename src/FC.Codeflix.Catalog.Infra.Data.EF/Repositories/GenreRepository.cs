using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class GenreRepository
    : IGenreRepository
{
    private readonly CodeflixCatalogDbContext _context;
    private DbSet<Genre> _genres
        => _context.Set<Genre>();

    private DbSet<GenresCategories> _genresCategories
        => _context.Set<GenresCategories>();

    public GenreRepository(CodeflixCatalogDbContext context)
        => _context = context;

    public async Task Insert(
        Genre genre, 
        CancellationToken cancellationToken
    ) {
        await _genres.AddAsync(genre);
        if (genre.Categories.Count > 0)
        {
            var relations = genre.Categories
                .Select(categoryId => new GenresCategories(
                    categoryId,
                    genre.Id
                ));
            await _genresCategories.AddRangeAsync(relations);
        }
    }

    public async Task<Genre> Get(
        Guid id, 
        CancellationToken cancellationToken
    )
    {
        var genre = await _genres.FindAsync(id);
        var categoryIds = await _genresCategories
            .Where(x => x.GenreId == genre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync();
        categoryIds.ForEach(genre.AddCategory);
        return genre;
    }

    public Task Delete(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Genre aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
