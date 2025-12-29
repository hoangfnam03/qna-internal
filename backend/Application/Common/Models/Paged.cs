namespace Application.Common.Models
{
    public class Paged<T>
    {
        public int Page { get; }
        public int PageSize { get; }
        public int Total { get; }
        public IReadOnlyList<T> Items { get; }

        public Paged(IReadOnlyList<T> items, int page, int pageSize, int total)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            Total = total;
        }
    }
}
