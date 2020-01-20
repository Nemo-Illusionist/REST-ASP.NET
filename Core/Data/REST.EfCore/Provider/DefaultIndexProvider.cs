using Microsoft.EntityFrameworkCore.Metadata.Builders;
using REST.EfCore.Contract;

namespace REST.EfCore.Provider
{
    internal class DefaultIndexProvider : IIndexProvider
    {
        public IndexBuilder HasMethod(IndexBuilder indexBuilder, string attributeMethod)
        {
            return indexBuilder;
        }
    }
}