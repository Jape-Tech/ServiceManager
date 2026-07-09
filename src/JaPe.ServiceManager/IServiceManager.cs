namespace JaPe.ServiceManager;

public interface IServiceManager
{
   public void Install(string serviceName, string displayName, string binaryPath, ServiceStartType startType); 
}