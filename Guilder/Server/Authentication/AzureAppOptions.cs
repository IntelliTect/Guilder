namespace Guilder.Server.Authentication
{
    public class AzureAppOptions
    {
        public const string SectionName = nameof(AzureAppOptions);
        public string? ClientSecret { get; set; }
        public string? ClientId { get; set; }
        public string? TenantId { get; set; }

    }
}
