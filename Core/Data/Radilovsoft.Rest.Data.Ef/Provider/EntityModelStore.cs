using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Radilovsoft.Rest.Data.Ef.Contract;

namespace Radilovsoft.Rest.Data.Ef.Provider
{
    [UsedImplicitly]
    public class EntityModelStore : IModelStore
    {
        private readonly Type[] _modelTypes;

        protected EntityModelStore([NotNull] Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            _modelTypes = assembly.GetExportedTypes()
                .Where(x => !x.IsInterface && x.GetCustomAttribute<TableAttribute>() != null)
                .ToArray();
        }

        public IEnumerable<Type> GetModels()
        {
            return _modelTypes;
        }
    }
}