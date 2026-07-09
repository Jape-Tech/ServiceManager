using System.Runtime.Versioning;
using JaPe.ServiceManager.Native;


namespace JaPe.ServiceManager;


[SupportedOSPlatform("windows5.1.2600")]
public sealed class ServiceManager:IServiceManager
{
    public void Install(ServiceDefinition definition)
    {
        using var scm = ServiceControlManager.Open();
        scm.CreateService(definition);
    }
}
