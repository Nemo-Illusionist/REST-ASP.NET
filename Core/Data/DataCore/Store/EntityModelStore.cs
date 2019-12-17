using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataCore.EntityContract;
using JetBrains.Annotations;

namespace DataCore.Store
{
    [UsedImplicitly]
    public abstract class EntityModelStore : IModelStore
    {
        private readonly Type[] _modelTypes;

        protected EntityModelStore([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            _modelTypes = assembly.GetExportedTypes()
                .Where(x => !x.IsInterface && typeof(IEntity).IsAssignableFrom(x))
                .ToArray();
        }

        public IEnumerable<Type> GetModels()
        {
            return _modelTypes;
        }
    }
}