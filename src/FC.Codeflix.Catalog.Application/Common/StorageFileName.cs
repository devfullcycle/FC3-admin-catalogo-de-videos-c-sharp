using FC.Codeflix.Catalog.Application.UseCases.CastMember.CreateCastMember;

namespace FC.Codeflix.Catalog.Application.Common;

public static class StorageFileName
{
    public static string Create(Guid id, string propertyName, string extension)
        => $"{id}-{propertyName.ToLower()}.{extension.Replace(".", "")}";
}
