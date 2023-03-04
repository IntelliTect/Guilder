using System.ComponentModel.DataAnnotations;

namespace Guilder.Server.Authentication;

public class AzureAppOptions
{
    public const string SectionName = nameof(AzureAppOptions);
    [Required]
    public string? ClientSecret { get; set; }
    [Required]
    public string? ClientId { get; set; }
    [Required]
    public string? TenantId { get; set; }

}
