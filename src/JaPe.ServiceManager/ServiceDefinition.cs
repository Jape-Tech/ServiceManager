namespace JaPe.ServiceManager;

public sealed record ServiceDefinition
{
    public required string Name {get; init;}
    public string? DisplayName {get; init;}
    public required string BinaryPath {get; init;}
    public ServiceStartType StartType {get; init;} = ServiceStartType.Automatic;
}