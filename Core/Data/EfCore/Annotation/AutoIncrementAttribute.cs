using System;

namespace EfCore.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AutoIncrementAttribute : Attribute
    {
    }
}