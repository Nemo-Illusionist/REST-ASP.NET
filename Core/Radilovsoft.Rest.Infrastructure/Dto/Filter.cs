namespace Radilovsoft.Rest.Infrastructure.Dto
{
    public class Filter
    {
        public Filter Left { get; set; }
        public Filter Right { get; set; }

        public string Operator { get; set; }

        public string Field { get; set; }
        public object Value { get; set; }

        public static Filter operator &(Filter f1, Filter f2)
        {
            return new Filter
            {
                Left = f1,
                Right = f2,
                Operator = GroupOperatorType.And.ToString()
            };
        }

        public static Filter operator |(Filter f1, Filter f2)
        {
            return new Filter
            {
                Left = f1,
                Right = f2,
                Operator = GroupOperatorType.Or.ToString()
            };
        }
    }
}