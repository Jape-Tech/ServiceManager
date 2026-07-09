using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Services;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;


namespace JaPe.ServiceManager;


[SupportedOSPlatform("windows5.1.2600")]
public sealed class ServiceManager:IServiceManager
{
    private const uint ScManagerCreateService = 0x0002;
    public void Install(
        string serviceName,
        string displayName,
        string binaryPath,
        ServiceStartType startType = ServiceStartType.Automatic)
    {
        ArgumentException.ThrowIfNullOrEmpty(serviceName);
        ArgumentException.ThrowIfNullOrEmpty(displayName);
        ArgumentException.ThrowIfNullOrEmpty(binaryPath);

        using var scmHandle = PInvoke.OpenSCManager(
            lpMachineName: null,
            lpDatabaseName: null,
            dwDesiredAccess: ScManagerCreateService);
        
        if (scmHandle.IsInvalid) ThrowLastWin32Error("Failed to open Service Control Manager");
        
    }
    private static SERVICE_START_TYPE MapStartType(ServiceStartType startType) => startType switch
    {
        ServiceStartType.Boot => SERVICE_START_TYPE.SERVICE_BOOT_START,
        ServiceStartType.System => SERVICE_START_TYPE.SERVICE_SYSTEM_START,
        ServiceStartType.Automatic => SERVICE_START_TYPE.SERVICE_AUTO_START,
        ServiceStartType.Manual => SERVICE_START_TYPE.SERVICE_DEMAND_START,
        ServiceStartType.Disabled => SERVICE_START_TYPE.SERVICE_DISABLED,
        _ => throw new ArgumentOutOfRangeException(nameof(startType), startType, null)
    };

    private static void ThrowLastWin32Error(string message) => throw new Win32Exception(
        Marshal.GetLastWin32Error(), message);
}
