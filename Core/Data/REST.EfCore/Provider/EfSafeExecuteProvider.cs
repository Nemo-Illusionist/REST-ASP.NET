using System;
using JetBrains.Annotations;
using REST.DataCore.Contract;
using REST.DataCore.Contract.Provider;
using REST.DataCore.Provider;
using REST.EfCore.Context;

namespace REST.EfCore.Provider
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