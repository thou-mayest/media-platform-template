namespace SharedKernal.Configurations;

public sealed class CorsOptions
{
    public const string CorsFrontendPolicyName = "frontend-cors";
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } = [];
}
