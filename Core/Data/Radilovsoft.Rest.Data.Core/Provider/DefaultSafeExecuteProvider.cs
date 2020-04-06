using System;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Provider;

namespace Radilovsoft.Rest.Data.Core.Provider
{
    public class DefaultSafeExecuteProvider : BaseSafeExecuteProvider
    {
        private readonly IDataProvider _dataProvider;

        public DefaultSafeExecuteProvider(
            IDataProvider dataProvider,
            IDataExceptionManager dataExceptionManager)
            : base(dataExceptionManager)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        protected override void Reset()
        {
        }

        protected override IDataProvider GetProvider() => _dataProvider;
    }
}