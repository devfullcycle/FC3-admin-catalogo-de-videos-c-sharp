using FC.Codeflix.Catalog.Application.UseCases.Video.DeleteVideo;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using System;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.DeleteVideo;

[CollectionDefinition(nameof(DeleteVideoTestFixture))]
public class DeleteVideoTestFixtureCollection
    : ICollectionFixture<DeleteVideoTestFixture>
{ }

public class DeleteVideoTestFixture : VideoTestFixtureBase
{
    internal DeleteVideoInput GetValidInput(Guid? id = null) 
        => new(id ?? Guid.NewGuid());
}
