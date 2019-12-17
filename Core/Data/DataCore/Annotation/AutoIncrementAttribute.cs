using System;

namespace DataCore.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AutoIncrementAttribute : Attribute
    {
    }
}