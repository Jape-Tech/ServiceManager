using JaPe.ServiceManager.Enums;

namespace JaPe.ServiceManager.Models;

public sealed record ServiceDefinition
{
    public required string Name {get; init;}
    public string? DisplayName {get; init;}
    public string? Description {get; init;}
    public string? ServiceUser {get; init;}
    public string? Password {get; init;}
    public IReadOnlyDictionary<string,string>? Environment {get; init;}
    public required string BinaryPath {get; init;}
    public ServiceStartType StartType {get; init;} = ServiceStartType.Automatic;
    
    public IReadOnlyCollection<string>? StartStopUsers {get; set;}
}