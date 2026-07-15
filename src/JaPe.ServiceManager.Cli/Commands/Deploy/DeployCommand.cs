using System.CommandLine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using JaPe.ServiceManager.Cli.Models;
using JaPe.ServiceManager.Models;

namespace JaPe.ServiceManager.Cli.Commands.Deploy;

internal sealed class DeployCommand : Command
{
    private readonly Argument<FileInfo> _servicesFile = new ("file")
    {
        Description = "Path to the services definition file"
    };
    
    private readonly Option<bool> _skipExisting = new ("--skip-existing")
    {
        Description = "Skip existing services"
    };

    public DeployCommand() 
        : base("deploy", "Deploys services from a YAML file")
    {
        Add(_servicesFile);
        Add(_skipExisting);
        SetAction(Execute);
    }

    private int Execute(ParseResult parseResult)
    {
        var file = parseResult.GetValue(_servicesFile)
            ?? throw new ArgumentException("Services file is required");
        var skipExisting = parseResult.GetValue(_skipExisting);
        
        if (!file.Exists)
        {
            Console.WriteLine($"File {file.FullName} does not exist");
            return 1;
        }
        
        var yaml = File.ReadAllText(file.FullName);
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        ServiceYamlDefinitionFile definitionFile;

        try
        {
            definitionFile = deserializer.Deserialize<ServiceYamlDefinitionFile>(yaml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deserializing YAML file '{file.FullName}': {ex.Message}");
            return 1;
        }
        
        if (definitionFile.Services.Count == 0)
        {
            Console.WriteLine($"No services defined in YAML file '{file.FullName}'");
            return 1;
        }
        
        var serviceManager = new ServiceManager();
        
        foreach (var service in definitionFile.ToServiceDefinitions())
        {
            if (string.IsNullOrWhiteSpace(service.Name))
            {
                Console.Error.WriteLine($"Service '{service.Name}' has no name defined");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(service.BinaryPath))
            {
                Console.Error.WriteLine($"Service '{service.Name}' has no binary path defined");
            }

            if (serviceManager.ServiceExists(service.Name))
            {
                if (skipExisting)
                {
                    Console.WriteLine($"Service '{service.Name}' already exists, skipping");
                    continue;
                }
                
                Console.Error.WriteLine($"Service '{service.Name}' already exists");
                return 1;
            }
            
            Console.WriteLine($"Installing service '{service.Name}'...");
                
            serviceManager.Install(service);
        }

        return 0;
    }
}