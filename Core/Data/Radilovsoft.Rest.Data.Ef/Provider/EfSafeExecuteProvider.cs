using System;
using JetBrains.Annotations;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Provider;
using Radilovsoft.Rest.Data.Core.Provider;
using Radilovsoft.Rest.Data.Ef.Context;

namespace Radilovsoft.Rest.Data.Ef.Provider
{
    public class EfSafeExecuteProvider : BaseSafeExecuteProvider
    {
        private readonly IDataProvider _dataProvider;
        private readonly ResetDbContext _dbContext;

        public EfSafeExecuteProvider(
            [NotNull] IDataProvider dataProvider,
            [NotNull] ResetDbContext dbContext,
            [NotNull] IDataExceptionManager dataExceptionManager) : base(dataExceptionManager)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected override void Reset()
        {
            _dbContext.Reset();
        }

        protected override IDataProvider GetProvider()
        {
            return _dataProvider;
        }
    }
}