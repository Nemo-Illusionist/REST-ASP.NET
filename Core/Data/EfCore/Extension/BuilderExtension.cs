using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Core.Comparer;
using EfCore.Annotation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Extension
{
    public static class BuilderExtension
    {
        public static ModelBuilder BuildEntity([NotNull] this ModelBuilder builder,
            [NotNull] IEnumerable<Type> modelTypes)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelTypes == null) throw new ArgumentNullException(nameof(modelTypes));

            foreach (var modelType in modelTypes)
            {
                builder.Entity(modelType);
            }

            return builder;
        }

        public static ModelBuilder BuildIndex([NotNull] this ModelBuilder builder,
            [NotNull] IEnumerable<Type> modelTypes)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelTypes == null) throw new ArgumentNullException(nameof(modelTypes));

            foreach (var type in modelTypes)
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

                    builder.Entity(type)
                        .HasIndex(index.Select(x => x.Name).ToArray())
                        .HasName(attribute.IndexName)
                        .IsUnique(attribute.IsUnique);
                }
            }

            return builder;
        }

        public static ModelBuilder BuildAutoIncrement([NotNull] this ModelBuilder builder,
            [NotNull] IEnumerable<Type> modelTypes)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (modelTypes == null) throw new ArgumentNullException(nameof(modelTypes));

            foreach (var type in modelTypes)
            {
                var properties = GetPropAttribute<AutoIncrementAttribute>(type);
                foreach (var property in properties)
                {
                    builder.Entity(type).Property(property.Property.Name).ValueGeneratedNever();
                }
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