using JaPe.ServiceManager.Cli.Commands;
 
var root = new CliRootCommand();
return root.Parse(args).Invoke();