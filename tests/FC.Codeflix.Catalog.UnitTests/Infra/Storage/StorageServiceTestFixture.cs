using FC.Codeflix.Catalog.UnitTests.Common;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Infra.Storage;

[CollectionDefinition(nameof(StorageServiceTestFixture))]
public class StorageServiceTestFixtureCollection
    : ICollectionFixture<StorageServiceTestFixture>
{
}

public class StorageServiceTestFixture : BaseFixture
{
    public string GetBucketName()
        => "fc3-catalog-medias";

    public string GetFileName()
        => Faker.System.CommonFileName();

    public string GetContentFile()
        => Faker.Lorem.Paragraph();

    public string GetContentType()
        => Faker.System.MimeType();
}
