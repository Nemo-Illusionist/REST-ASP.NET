using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Radilovsoft.Rest.Data.Ef.Contract
{
    public interface IIndexProvider
    {
        IndexBuilder HasMethod(IndexBuilder indexBuilder, string attributeMethod);
    }
}