using System;
using System.Globalization;

namespace REST.Core.Extension
{
    public static class StringExtension
    {
        public static string ToUpperCaseFirstChar(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            Span<char> span = stackalloc char[input.Length];
            input.AsSpan().CopyTo(span);
            span[0] = char.ToUpper(span[0], CultureInfo.InvariantCulture);
            return new string(span);
        }
        
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}