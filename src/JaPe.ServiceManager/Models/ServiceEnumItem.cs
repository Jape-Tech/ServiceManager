namespace JaPe.ServiceManager.Models;

public record ServiceEnumItem
{
    public required string Name { get; init; }
    public string? DisplayName { get; init; }
    public string? Status { get; init; }
};