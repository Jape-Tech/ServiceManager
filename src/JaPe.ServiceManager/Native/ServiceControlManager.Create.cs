using Windows.Win32;
using Windows.Win32.System.Services;

namespace JaPe.ServiceManager.Native;
public sealed partial class ServiceControlManager
{
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
}