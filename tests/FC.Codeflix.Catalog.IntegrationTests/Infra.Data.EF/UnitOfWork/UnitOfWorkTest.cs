using System.Threading.Tasks;
using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF;
using Xunit;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using FluentAssertions;
using FC.Codeflix.Catalog.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork;

[Collection(nameof(UnitOfWorkTestFixture))]
public class UnitOfWorkTest
{
    private readonly UnitOfWorkTestFixture _fixture;

    public UnitOfWorkTest(UnitOfWorkTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(Commit))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Commit()
    {
        var dbContext = _fixture.CreateDbContext();
        var examplecategoriesList = _fixture.GetExampleCategoriesList();
        await dbContext.AddRangeAsync(examplecategoriesList);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWorkInfra.UnitOfWork>>());

        await unitOfWork.Commit(CancellationToken.None);

        var assertDbContext = _fixture.CreateDbContext(true);
        var savedCategories = assertDbContext.Categories
            .AsNoTracking().ToList();
        savedCategories.Should()
            .HaveCount(examplecategoriesList.Count);
    }


    [Fact(DisplayName = nameof(Rollback))]
    [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
    public async Task Rollback()
    {
        var dbContext = _fixture.CreateDbContext();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var eventPublisher = new DomainEventPublisher(serviceProvider);
        var unitOfWork = new UnitOfWorkInfra.UnitOfWork(dbContext,
            eventPublisher,
            serviceProvider.GetRequiredService<ILogger<UnitOfWorkInfra.UnitOfWork>>());

        var task = async () 
            => await unitOfWork.Rollback(CancellationToken.None);

        await task.Should().NotThrowAsync();
    }
}
