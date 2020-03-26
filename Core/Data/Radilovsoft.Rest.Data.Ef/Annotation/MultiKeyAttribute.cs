using System;

namespace Radilovsoft.Rest.Data.Ef.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class MultiKeyAttribute : Attribute
    {
    }
}