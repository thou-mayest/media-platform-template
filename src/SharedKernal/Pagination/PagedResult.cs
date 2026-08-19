namespace SharedKernal.Pagination
{
   
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize,
        int TotalItems)
    {
        public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPrev => Page > 1;
        public bool HasNext => Page < TotalPages;

        public static PagedResult<T> Empty(PageRequest request) =>
            new(Array.Empty<T>(), request.Page, request.PageSize, 0);

        public PagedResult<TOut> Map<TOut>(Func<T, TOut> selector) =>
            new(Items.Select(selector).ToList(), Page, PageSize, TotalItems);
    }
}