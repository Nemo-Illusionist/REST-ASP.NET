using System;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Provider;
using Radilovsoft.Rest.Data.Ef.Context;

namespace Radilovsoft.Rest.Data.Ef.Provider
{
    public class EfSafeExecuteProvider : SafeExecuteProvider
    {
        private readonly ResetDbContext _dbContext;

        public EfSafeExecuteProvider(
            ResetDbContext dbContext,
            IDataExceptionManager dataExceptionManager) : base(dataExceptionManager)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        protected override void Reset()
        {
            _dbContext.Reset();
        }
    }
}