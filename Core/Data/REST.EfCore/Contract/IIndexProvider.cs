using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace REST.EfCore.Contract
{
    public interface IIndexProvider
    {
        IndexBuilder HasMethod(IndexBuilder indexBuilder, string attributeMethod);
    }
}