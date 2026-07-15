using System.Runtime.Versioning;
using JaPe.ServiceManager.Enums;
using JaPe.ServiceManager.Native;
using JaPe.ServiceManager.Models;


namespace JaPe.ServiceManager;


[SupportedOSPlatform("windows5.1.2600")]
public sealed class ServiceManager:IServiceManager
{
    public void Install(ServiceDefinition definition)
    {
        using var scm = ServiceControlManager.Open(NativeConstants.ScManagerCreateService);
        scm.CreateService(definition);
    }

    public List<ServiceEnumItem> GetServices()
    {
        using var scm = ServiceControlManager.Open(NativeConstants.ScManagerEnumerateService);
        return scm.EnumServices();
    }
    
    public List<ServiceEnumItem> GetServices(string pattern)
    {
        using var scm = ServiceControlManager.Open(NativeConstants.ScManagerEnumerateService);
        return scm.EnumServices().Where(s=>s.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    public bool ServiceExists(string name)
    {
        using var scm = ServiceControlManager.Open(NativeConstants.ScManagerEnumerateService);
        return scm.EnumServices().Any(s=>s.Name==name);
    }
    
    public ServiceDefinition ServiceInfo(string name)
    {
        var info =  ServiceRegistryStore.Read(name);
        using var scm = ServiceControlManager.Open(NativeConstants.ScManagerEnumerateService);
        info.StartStopUsers = scm.GetStartStopUsers(name);
        return info;
    }
}
