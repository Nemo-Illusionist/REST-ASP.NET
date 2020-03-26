using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Radilovsoft.Rest.Data.Ef.Contract;

namespace Radilovsoft.Rest.Data.Ef.Provider
{
    internal class DefaultIndexProvider : IIndexProvider
    {
        public IndexBuilder HasMethod(IndexBuilder indexBuilder, string attributeMethod)
        {
            return indexBuilder;
        }
    }
}