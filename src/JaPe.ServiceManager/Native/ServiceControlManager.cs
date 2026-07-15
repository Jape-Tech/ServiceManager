using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.System.Services;
using JaPe.ServiceManager.Enums;

namespace JaPe.ServiceManager.Native;
[SupportedOSPlatform("windows5.1.2600")]
internal sealed partial class ServiceControlManager : IDisposable
{
    
    private readonly CloseServiceHandleSafeHandle _scmHandle;

    private ServiceControlManager(CloseServiceHandleSafeHandle scmHandle)
    {
        _scmHandle = scmHandle;
    }
    public static ServiceControlManager Open(uint desiredAccess = NativeConstants.ScManagerCreateService)
    {
        var handle = PInvoke.OpenSCManager(
            lpMachineName: null,
            lpDatabaseName: null,
            dwDesiredAccess: desiredAccess); 
        
        if (handle.IsInvalid) ThrowLastWin32Error("Failed to open Service Control Manager");
        
        return new ServiceControlManager(handle);
    }


    public void Dispose()
    {
        _scmHandle.Dispose();
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
    private static void ThrowLastWin32Error(string message) => 
        throw new Win32Exception(Marshal.GetLastWin32Error(), message);
    
}