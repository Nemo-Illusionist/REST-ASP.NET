namespace Radilovsoft.Rest.Infrastructure.Contract.Dto
{
    public interface IPageFilter
    {
        public int Page { get; }
        
        public int PageSize { get; }
    }
}