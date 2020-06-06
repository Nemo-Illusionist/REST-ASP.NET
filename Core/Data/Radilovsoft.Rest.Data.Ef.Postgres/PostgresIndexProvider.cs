using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Radilovsoft.Rest.Data.Ef.Contract;

namespace Radilovsoft.Rest.Data.Ef.Postgres
{
    public class PostgresIndexProvider : IIndexProvider
    {
        public IndexBuilder HasMethod(IndexBuilder indexBuilder, string attributeMethod)
        {
            return indexBuilder.HasMethod(attributeMethod);
        }
    }
}