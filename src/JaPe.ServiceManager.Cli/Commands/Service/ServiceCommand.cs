using System.CommandLine;

namespace JaPe.ServiceManager.Cli.Commands.Service;

public class ServiceCommand:Command
{
    public ServiceCommand()
        : base("service", "Manage Windows services.")
    {
       Subcommands.Add(new ServiceListCommand());
    }
    
}