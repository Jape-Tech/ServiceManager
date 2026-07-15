using JaPe.ServiceManager.Models;
namespace JaPe.ServiceManager;

public interface IServiceManager
{
   public void Install(ServiceDefinition definition);
   public List<ServiceEnumItem> GetServices();
   public List<ServiceEnumItem> GetServices(string pattern);
   public bool ServiceExists(string name);
   public ServiceDefinition ServiceInfo(string name);
}