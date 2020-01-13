using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace REST.EfCore.Provider
{
    public interface IIndexManager
    {
        IndexBuilder HasMethod(IndexBuilder indexBuilder, string attributeMethod);
    }
}