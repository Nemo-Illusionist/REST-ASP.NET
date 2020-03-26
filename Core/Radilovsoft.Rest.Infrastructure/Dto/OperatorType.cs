using JetBrains.Annotations;

namespace Radilovsoft.Rest.Infrastructure.Dto
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