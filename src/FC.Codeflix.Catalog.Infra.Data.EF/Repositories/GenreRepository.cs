using FC.Codeflix.Catalog.Application.Exceptions;
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
        var genre = await _genres
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        NotFoundException.ThrowIfNull(genre, $"Genre '{id}' not found.");
        var categoryIds = await _genresCategories
            .Where(x => x.GenreId == genre.Id)
            .Select(x => x.CategoryId)
            .ToListAsync(cancellationToken);
        categoryIds.ForEach(genre.AddCategory);
        return genre;
    }

    public Task Delete(
        Genre aggregate, 
        CancellationToken cancellationToken
    )
    {
        _genresCategories.RemoveRange(
            _genresCategories.Where(x => x.GenreId == aggregate.Id)
        );
        _genres.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task Update(Genre genre, CancellationToken cancellationToken)
    {
        _genres.Update(genre);
        _genresCategories.RemoveRange(_genresCategories
            .Where(x => x.GenreId == genre.Id));
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

    public async Task<SearchOutput<Genre>> Search(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = _genres.AsNoTracking();

        query = AddOrderToQuery(query, input.OrderBy, input.Order);

        if(!String.IsNullOrWhiteSpace(input.Search))
            query = query.Where(genre => genre.Name.Contains(input.Search));

        var genres = await query
                .Skip(toSkip).Take(input.PerPage).ToListAsync();

        var total = await query.CountAsync();

        var genresIds = genres.Select(genre => genre.Id).ToList();
        var relations = await _genresCategories
            .Where(relation => genresIds.Contains(relation.GenreId))
            .ToListAsync();
        var relationsByGenreIdGroup = 
            relations.GroupBy(x => x.GenreId).ToList();
        relationsByGenreIdGroup.ForEach(relationGroup => {
            var genre = genres.Find(genre => genre.Id == relationGroup.Key);
            if (genre is null) return;
            relationGroup.ToList()
                .ForEach(relation => genre.AddCategory(relation.CategoryId));
        });
        
        return new SearchOutput<Genre>(
            input.Page,
            input.PerPage,
            total,
            genres
        );
    }

    private IQueryable<Genre> AddOrderToQuery(
        IQueryable<Genre> query,
        string orderProperty,
        SearchOrder order
    )
    {
        var orderedQuery = (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name)
                .ThenBy(x => x.Id),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name)
                .ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
        };
        return orderedQuery;
    }
}
