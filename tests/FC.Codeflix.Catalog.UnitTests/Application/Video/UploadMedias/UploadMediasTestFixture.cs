using UseCase = FC.Codeflix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.Codeflix.Catalog.UnitTests.Common.Fixtures;
using System;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UploadMedias;

[CollectionDefinition(nameof(UploadMediasTestFixture))]
public class UploadMediasTestFixtureCollection 
    : ICollectionFixture<UploadMediasTestFixture>
{ }

public class UploadMediasTestFixture : VideoTestFixtureBase
{
    public UseCase.UploadMediasInput GetValidInput()
        => new(
            Guid.NewGuid(),
            GetValidMediaFileInput(),
            GetValidMediaFileInput()
        );
}
