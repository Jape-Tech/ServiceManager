using System.CommandLine;
using JaPe.ServiceManager.Cli.Commands.Service;
using JaPe.ServiceManager.Cli.Commands.Deploy;

namespace JaPe.ServiceManager.Cli.Commands;

internal sealed class CliRootCommand : RootCommand
{
    public CliRootCommand()
        : base("JaPe Service Manager - Windows service management made less painful.")
    {
        Subcommands.Add(new ServiceCommand());
        Subcommands.Add(new DeployCommand());
    }
}