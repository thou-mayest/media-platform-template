namespace Profiles.Presentation.Common;


public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPrev,
    bool HasNext);