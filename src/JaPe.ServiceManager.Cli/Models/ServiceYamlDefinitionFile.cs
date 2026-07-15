using JaPe.ServiceManager.Enums;
using JaPe.ServiceManager.Models;
namespace JaPe.ServiceManager.Cli.Models;

internal sealed record ServiceYamlDefinitionFile
{
    public List<ServiceYamlDefinition> Services { get; init; } = [];

    public IReadOnlyCollection<ServiceDefinition> ToServiceDefinitions()
    {
        var serviceDefinitions = new List<ServiceDefinition>();
        foreach (var service in Services)
        {
            serviceDefinitions.Add(new ServiceDefinition
            {
                Name = service.Name,
                DisplayName = service.DisplayName,
                Description = service.Description,
                ServiceUser = service.ServiceUser,
                Environment = service.Environment,
                BinaryPath = GetBinaryPath(service),
                StartType = service.StartType,
                StartStopUsers = service.StartStopUsers
            });
        }
        return serviceDefinitions;
    }
    
    private static string GetBinaryPath(ServiceYamlDefinition service)
    {
        if (!string.IsNullOrWhiteSpace(service.BinaryPath))
        {
            return service.BinaryPath;
        }
        
        if (string.IsNullOrWhiteSpace(service.ExePath))
        {
            throw new ArgumentException("Either BinaryPath or ExePath must be specified");
        }
        
        var binaryPath = $"\"{service.ExePath}\"";
        if (service.Arguments is not null && service.Arguments.Count > 0)
        {
            if (service.ArgsAsSingleString) 
            {
                binaryPath += $" \"{string.Join(" ", service.Arguments)}\"";
            }
            else
            {
                binaryPath += $" {string.Join(" ", service.Arguments)}";
            }
        } 
        return binaryPath;
    }
};

internal sealed record ServiceYamlDefinition
{
    public required string Name { get; init; }
    public string? DisplayName { get; init; } 
    public string? Description { get; init; }
    public string? ServiceUser { get; init; }
    public string? Password { get; init; }
    public Dictionary<string, string>? Environment { get; init; }
    public string? BinaryPath { get; init; }
    
    public string? ExePath { get; init; }
    public List<string>? Arguments { get; init; }
    public bool ArgsAsSingleString { get; init; } = true;
    public ServiceStartType StartType { get; init; } = ServiceStartType.Automatic;
    public List<string>? StartStopUsers { get; init; }
};