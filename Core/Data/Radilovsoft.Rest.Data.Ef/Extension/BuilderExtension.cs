using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Radilovsoft.Rest.Data.Ef.Annotation;
using Radilovsoft.Rest.Data.Ef.Contract;
using Radilovsoft.Rest.Data.Ef.Provider;
using Radilovsoft.Rest.Core.Comparer;

namespace Radilovsoft.Rest.Data.Ef.Extension
{
    public static class BuilderExtension
    {
        public static ModelBuilder BuildEntity(this ModelBuilder builder, IModelStore modelStore)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelStore == null) throw new ArgumentNullException(nameof(modelStore));

            foreach (var modelType in modelStore.GetModels())
            {
                builder.Entity(modelType);
            }

            return builder;
        }

        public static ModelBuilder BuildIndex(
            this ModelBuilder builder,
            IModelStore modelStore,
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
                    .GroupBy(p => p.IndexName, NullUniqueEqualityComparer<string>.Instance);

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

        public static ModelBuilder BuildAutoIncrement(this ModelBuilder builder, IModelStore modelStore)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelStore == null) throw new ArgumentNullException(nameof(modelStore));

            foreach (var type in modelStore.GetModels())
            {
                var properties = GetPropAttribute<AutoIncrementAttribute>(type);
                foreach (var property in properties)
                {
                    switch (property.Attribute.Type)
                    {
                        case AutoIncrementType.Add:
                            builder.Entity(type).Property(property.Property.Name).ValueGeneratedOnAdd();
                            break;
                        case AutoIncrementType.Update:
                            builder.Entity(type).Property(property.Property.Name).ValueGeneratedOnUpdate();
                            break;
                        case AutoIncrementType.AddOrUpdate:
                            builder.Entity(type).Property(property.Property.Name).ValueGeneratedOnAddOrUpdate();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return builder;
        }

        public static ModelBuilder BuildMultiKey(this ModelBuilder builder, IModelStore modelStore)
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
                .Where(p => p.Property != null && p.Attribute != null);
        }
    }
}