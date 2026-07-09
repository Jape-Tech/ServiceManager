using System.CommandLine;
using JaPe.ServiceManager.Cli.Commands.Service;

namespace JaPe.ServiceManager.Cli.Commands;

internal sealed class CliRootCommand : RootCommand
{
    public CliRootCommand()
        : base("JaPe Service Manager - Windows service management made less painful.")
    {
        Subcommands.Add(new ServiceCommand());
    }
}