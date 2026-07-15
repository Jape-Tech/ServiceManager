using System.CommandLine;
using JaPe.ServiceManager.Models;

namespace JaPe.ServiceManager.Cli.Commands.Service;

internal sealed class ServiceListCommand : Command
{

   private readonly Option<bool> _jsonOption = new(name: "--json")
   {
      Description = "Output in JSON format."
   };
   
   public ServiceListCommand()
      : base("list", "List services.")
   {
      Add(_jsonOption);
      SetAction(Execute);
   }
   
   private int Execute(ParseResult parseResult)
   {
      var jpsm = new ServiceManager();
      var services = jpsm.GetServices();
      var json = parseResult.GetValue<bool>(_jsonOption);
      if (json)
      {
         parseResult.InvocationConfiguration.Output.WriteLine("[]");
      }
      else 
      {
         foreach (var service in services)
         {
            parseResult.InvocationConfiguration.Output.WriteLine(service.Name);
         }
      }
      return 0;
   }
   
}