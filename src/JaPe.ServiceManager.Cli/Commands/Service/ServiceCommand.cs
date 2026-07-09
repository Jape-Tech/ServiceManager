using System.CommandLine;

namespace JaPe.ServiceManager.Cli.Commands.Service;

internal sealed class ServiceCommand : Command
{
    public ServiceCommand()
        : base("service", "Manage Windows services.")
    {
       Subcommands.Add(new ServiceListCommand());
    }
    
}