using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Radilovsoft.Rest.Data.Ef.Annotation;
using Radilovsoft.Rest.Data.Ef.Contract;
using Radilovsoft.Rest.Data.Ef.Provider;
using Radilovsoft.Rest.Core.Comparer;

namespace Radilovsoft.Rest.Data.Ef.Extension
{
    public static class BuilderExtension
    {
        public static ModelBuilder BuildEntity([NotNull] this ModelBuilder builder, [NotNull] IModelStore modelStore)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelStore == null) throw new ArgumentNullException(nameof(modelStore));

            foreach (var modelType in modelStore.GetModels())
            {
                builder.Entity(modelType);
            }

            return builder;
        }

        public static ModelBuilder BuildIndex([NotNull] this ModelBuilder builder, [NotNull] IModelStore modelStore,
            IIndexProvider indexProvider = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelStore == null) throw new ArgumentNullException(nameof(modelStore));
            indexProvider ??= new DefaultIndexProvider();

            foreach (var type in modelStore.GetModels())
            {
                var indices = GetPropAttribute<IndexAttribute>(type)
                    .Select(p => new
                    {
                        IndexName = string.IsNullOrWhiteSpace(p.Attribute.IndexName)
                            ? null
                            : p.Attribute.IndexName,
                        p.Property.Name,
                        p.Attribute
                    })
                    .GroupBy(p => p.IndexName, new NullUniqueEqualityComparer<string>());

                foreach (var index in indices)
                {
                    var attribute = index.First().Attribute;

                    var indexBuilder = builder.Entity(type)
                        .HasIndex(index.Select(x => x.Name).ToArray())
                        .HasName(attribute.IndexName)
                        .IsUnique(attribute.IsUnique);

                    if (!string.IsNullOrEmpty(attribute.Method))
                    {
                        indexProvider.HasMethod(indexBuilder, attribute.Method);
                    }
                }
            }

            return builder;
        }

        public static ModelBuilder BuildAutoIncrement([NotNull] this ModelBuilder builder,
            [NotNull] IModelStore modelStore)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelStore == null) throw new ArgumentNullException(nameof(modelStore));

            foreach (var type in modelStore.GetModels())
            {
                var properties = GetPropAttribute<AutoIncrementAttribute>(type);
                foreach (var property in properties)
                {
                    builder.Entity(type).Property(property.Property.Name).ValueGeneratedOnAdd();
                }
            }

            return builder;
        }

        public static ModelBuilder BuildMultiKey([NotNull] this ModelBuilder builder,
            [NotNull] IModelStore modelStore)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelStore == null) throw new ArgumentNullException(nameof(modelStore));

            foreach (var type in modelStore.GetModels())
            {
                var properties = GetPropAttribute<MultiKeyAttribute>(type)
                    .Select(p => p.Property.Name)
                    .ToArray();
                if (!properties.Any()) continue;
                builder.Entity(type).HasKey(properties);
            }

            return builder;
        }

        private static IEnumerable<(PropertyInfo Property, T Attribute )> GetPropAttribute<T>(Type type)
            where T : Attribute
        {
            return type.GetProperties()
                .Select(p => (Property: p, Attribute: p.GetCustomAttribute<T>(true)))
                .Where(x => x.Item2 != null);
        }
    }
}