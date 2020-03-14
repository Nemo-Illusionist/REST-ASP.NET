using System;

namespace REST.EfCore.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MultiKeyAttribute : Attribute
    {
    }
}