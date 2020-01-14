using JetBrains.Annotations;

namespace REST.Infrastructure.Dto
{
    [PublicAPI]
    public class Filter
    {
        public Filter Left { get; set; }
        public Filter Right { get; set; }

        public string Operator { get; set; }

        public string Field { get; set; }
        public object Value { get; set; }
    }
}