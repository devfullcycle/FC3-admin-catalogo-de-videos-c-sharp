using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.EndToEndTests.Base;
public class BaseFixture
{
    public BaseFixture()
    => Faker = new Faker("pt_BR");

    protected Faker Faker { get; set; }

    public CodeflixCatalogDbContext CreateDbContext()
    {
        var context = new CodeflixCatalogDbContext(
            new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
            .UseInMemoryDatabase("end2end-tests-db")
            .Options
        );
        return context;
    }
}
