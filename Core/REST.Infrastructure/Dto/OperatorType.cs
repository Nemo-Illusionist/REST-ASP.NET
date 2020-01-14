using JetBrains.Annotations;

namespace REST.Infrastructure.Dto
{
    [PublicAPI]
    public enum OperatorType
    {
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
    }
}