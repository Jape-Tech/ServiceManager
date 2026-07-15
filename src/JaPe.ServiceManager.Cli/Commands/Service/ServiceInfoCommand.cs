using System.CommandLine;

namespace JaPe.ServiceManager.Cli.Commands.Service;
internal sealed class ServiceInfoCommand: Command
{
    private readonly Option<string> _serviceNameOption = new(name: "--name")
    {
        Description = "The name of the service"
    };

    public ServiceInfoCommand()
    : base("info", "Get information about a service")
    {
        Add(_serviceNameOption);
        SetAction(Execute);
    }

    private int Execute(ParseResult parseResult)
    {
        var jpsm = new ServiceManager();
        var serviceName = parseResult.GetValue(_serviceNameOption);
        ArgumentException.ThrowIfNullOrEmpty(serviceName);
        var serviceInfo = jpsm.ServiceInfo(serviceName);
        
        Console.WriteLine($"Name: {serviceInfo.Name}");
        Console.WriteLine($"DisplayName: {serviceInfo.DisplayName}");
        Console.WriteLine($"Description: {serviceInfo.Description}");
        Console.WriteLine($"ServiceUser: {serviceInfo.ServiceUser}");
        Console.WriteLine($"Password: {serviceInfo.Password}");
        Console.WriteLine($"BinaryPath: {serviceInfo.BinaryPath}");
        Console.WriteLine($"StartType: {serviceInfo.StartType}");
        Console.WriteLine($"StartStopUsers: {string.Join(", ", serviceInfo.StartStopUsers ?? [])}");

        return 0;
    }
}