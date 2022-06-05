using FC.Codeflix.Catalog.EndToEndTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Genre.Common;
public class GenreBaseFixture
    : BaseFixture
{
    public GenrePersistence Persistence { get; set; }

    public GenreBaseFixture()
        : base()
    {
        Persistence = new GenrePersistence(CreateDbContext());
    }

    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public bool GetRandomBoolean()
    => new Random().NextDouble() < 0.5;

    public DomainEntity.Genre GetExampleGenre(
        bool? isActive = null,
        List<Guid>? categoriesIds = null,
        string? name = null
    )
    {
        var genre = new DomainEntity.Genre(
            name ?? GetValidGenreName(),
            isActive ?? GetRandomBoolean()
        );
        categoriesIds?.ForEach(genre.AddCategory);
        return genre;
    }

    public List<DomainEntity.Genre> GetExampleListGenres(int count = 10)
        => Enumerable
            .Range(1, count)
            .Select(_ => GetExampleGenre())
            .ToList();

}
