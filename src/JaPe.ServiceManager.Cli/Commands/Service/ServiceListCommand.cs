using System.CommandLine;
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
      var json = parseResult.GetValue<bool>(_jsonOption);
      if (json)
      {
         parseResult.InvocationConfiguration.Output.WriteLine("[]");
      }
      else 
      {
         parseResult.InvocationConfiguration.Output.WriteLine("Service listing is not yet implemented.");
      }
      return 0;
   }
   
}