namespace Storage.Application.Files;

internal static class FilesCacheKeys
{
    public static string Tag = "files";

    public static string AllFiles = "files:all";

    public static string GetFileKey(Guid id) => $"files:{id}";
}
