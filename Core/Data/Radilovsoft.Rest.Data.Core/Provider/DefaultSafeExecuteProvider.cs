using System;
using JetBrains.Annotations;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Provider;

namespace Radilovsoft.Rest.Data.Core.Provider
{
    public class DefaultSafeExecuteProvider : BaseSafeExecuteProvider
    {
        private readonly IDataProvider _dataProvider;

        public DefaultSafeExecuteProvider(
            [NotNull] IDataProvider dataProvider,
            [NotNull] IDataExceptionManager dataExceptionManager)
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