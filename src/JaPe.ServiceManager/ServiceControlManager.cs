using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Services;

namespace JaPe.ServiceManager;
[SupportedOSPlatform("windows5.1.2600")]
public sealed class ServiceControlManager : IDisposable
{
    
    private readonly CloseServiceHandleSafeHandle _scmHandle;

    private ServiceControlManager(CloseServiceHandleSafeHandle scmHandle)
    {
        _scmHandle = scmHandle;
    }
    public static ServiceControlManager Open()
    {
        var handle = PInvoke.OpenSCManager(
            lpMachineName: null,
            lpDatabaseName: null,
            dwDesiredAccess: NativeConstants.ScManagerCreateService);
        
        if (handle.IsInvalid) ThrowLastWin32Error("Failed to open Service Control Manager");
        
        return new ServiceControlManager(handle);
    }

    public void CreateService(ServiceDefinition definition)
    {
        ArgumentException.ThrowIfNullOrEmpty(definition.Name);
        ArgumentException.ThrowIfNullOrEmpty(definition.BinaryPath);

        uint tagId;
        
        using var serviceHandle = PInvoke.CreateService(
            hSCManager: _scmHandle,
            lpServiceName: definition.Name,
            lpDisplayName: definition.DisplayName ?? definition.Name,
            dwDesiredAccess: NativeConstants.ServiceAllAccess,
            dwServiceType: ENUM_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS,
            dwStartType: MapStartType(definition.StartType),
            dwErrorControl: SERVICE_ERROR.SERVICE_ERROR_NORMAL,
            lpBinaryPathName: definition.BinaryPath,
            lpLoadOrderGroup: null,
            lpdwTagId: out tagId,
            lpDependencies: null,
            lpServiceStartName: null,
            lpPassword: null
        );
        
        if (serviceHandle.IsInvalid) ThrowLastWin32Error($"Failed to create service '{definition.Name}'");
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