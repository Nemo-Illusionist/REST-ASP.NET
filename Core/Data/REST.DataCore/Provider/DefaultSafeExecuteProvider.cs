using System;
using JetBrains.Annotations;
using REST.DataCore.Contract;
using REST.DataCore.Contract.Provider;

namespace REST.DataCore.Provider
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