using System;

namespace Radilovsoft.Rest.Data.Ef.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AutoIncrementAttribute : Attribute
    {
        public AutoIncrementType Type { get; }

        public AutoIncrementAttribute(AutoIncrementType type)
        {
            Type = type;
        }

        public AutoIncrementAttribute()
        {
            Type = AutoIncrementType.Add;
        }
    }
}