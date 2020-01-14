using Microsoft.EntityFrameworkCore.Metadata.Builders;
using REST.EfCore.Contract;

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