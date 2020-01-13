using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace REST.EfCore.Provider
{
    internal class DefaultIndexManager : IIndexManager
    {
        public IndexBuilder HasMethod(IndexBuilder indexBuilder, string attributeMethod)
        {
            return indexBuilder;
        }
    }
}