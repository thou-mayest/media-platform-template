namespace SharedKernal.Pagination
{
   
    public sealed record PageRequest
    {
        public const int DefaultPageSize = 24;
        public const int MaxPageSize = 100;

        public int Page { get; }
        public int PageSize { get; }

        private PageRequest(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public static PageRequest Create(int? page = null, int? pageSize = null) =>
            new(Math.Max(1, page ?? 1),
                Math.Clamp(pageSize ?? DefaultPageSize, 1, MaxPageSize));

        public int Skip => (Page - 1) * PageSize;
    }
}