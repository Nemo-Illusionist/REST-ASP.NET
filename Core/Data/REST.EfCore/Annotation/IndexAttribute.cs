using System;

namespace REST.EfCore.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        public string IndexName { get; }

        public bool IsUnique { get; }
        public string Method { get; }

        public IndexAttribute(string indexName = null, bool isUnique = false, string method = null)
        {
            IndexName = indexName;
            IsUnique = isUnique;
            Method = method;
        }
    }
}