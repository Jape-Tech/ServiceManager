using System.Runtime.Versioning;


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
