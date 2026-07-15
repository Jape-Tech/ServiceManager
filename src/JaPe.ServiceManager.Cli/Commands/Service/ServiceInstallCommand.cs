using System.CommandLine;
using JaPe.ServiceManager.Models;

namespace JaPe.ServiceManager.Cli.Commands.Service;

internal sealed class ServiceInstallCommand: Command
{
   private readonly Option<string> _nameOption = new("--name")
   {
      Description = "The name of the service to create",
      Required = true
   };
   private readonly Option<string> _descriptionOption = new("--description")
   {
      Description = "The description of the service to create",
   };
   private readonly Option<string> _exeOption = new("--exe")
   {
      Description = "The path to the executable to run",
      Required = true
   };
   private readonly Option<string> _argsOption = new("--args")
   {
      Description = "The arguments to pass to the executable"
   };

   private readonly Option<string> _userOption = new("--user")
   {
      Description = "The user to run the service as"
   };
   private readonly Option<string> _passwordOption = new("--password")
   {
      Description = "The password for the user to run the service as"
   };
   private readonly Option<string[]> _startStopUsersOption = new("--start-stop-users")
   {
      Description = "The users to grant start/stop permissions to"
   };
   
   public ServiceInstallCommand() 
      : base("install", "Create a new service")
   {
      Add(_nameOption);
      Add(_descriptionOption);
      Add(_exeOption);
      Add(_argsOption);
      Add(_userOption);
      Add(_passwordOption);
      Add(_startStopUsersOption);
      SetAction(Execute);
   }

   private int Execute(ParseResult parseResult)
   {
      var serviceName = parseResult.GetValue(_nameOption) ?? throw new ArgumentException("Service name is required");
      var serviceDescription = parseResult.GetValue(_descriptionOption) ?? string.Empty;
      var serviceExe = parseResult.GetValue(_exeOption) ?? throw new ArgumentException("Service executable is required");
      var serviceArgs = parseResult.GetValue(_argsOption) ?? string.Empty;
      var serviceUser = parseResult.GetValue(_userOption) ?? string.Empty;
      var servicePassword = parseResult.GetValue(_passwordOption) ?? string.Empty;
      var serviceStartStopUsers = parseResult.GetValue(_startStopUsersOption) ?? Array.Empty<string>();
      
      var binaryPath = $"\"{serviceExe}\"";
      if (!string.IsNullOrEmpty(serviceArgs))
      {
         binaryPath += $" \"{serviceArgs}\"";
      }
      Console.WriteLine(binaryPath);
      var def = new ServiceDefinition
      {
         Name = serviceName,
         Description = serviceDescription,
         BinaryPath = binaryPath,
         ServiceUser = serviceUser,
         Password = servicePassword,
         StartStopUsers = serviceStartStopUsers
      };
      
      var jpsm = new ServiceManager();
      jpsm.Install(def);
      return 0;
   }
}