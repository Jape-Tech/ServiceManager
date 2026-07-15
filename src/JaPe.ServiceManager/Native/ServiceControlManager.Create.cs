using Windows.Win32;
using Windows.Win32.System.Services;
using JaPe.ServiceManager.Enums;
using JaPe.ServiceManager.Models;

namespace JaPe.ServiceManager.Native;
internal sealed partial class ServiceControlManager
{
    public void CreateService(ServiceDefinition definition)
    {
        ArgumentException.ThrowIfNullOrEmpty(definition.Name);
        ArgumentException.ThrowIfNullOrEmpty(definition.BinaryPath);

        
        using var serviceHandle = PInvoke.CreateService(
            hSCManager: _scmHandle,
            lpServiceName: definition.Name,
            lpDisplayName: definition.DisplayName ?? definition.Name,
            dwDesiredAccess: NativeConstants.ServiceAllAccess,
            dwServiceType: ENUM_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS,
            dwStartType: MapStartType(definition.StartType),
            dwErrorControl: SERVICE_ERROR.SERVICE_ERROR_NORMAL,
            lpBinaryPathName: definition.BinaryPath,
            lpDependencies: null,
            lpServiceStartName: definition.ServiceUser ?? null,
            lpPassword: definition.Password ?? null
            // Do not pass lpdwTagId unless lpLoadOrderGroup is specified.
            // Otherwise, CreateService fails with ERROR_INVALID_PARAMETER (87).
        );
        
        if (serviceHandle.IsInvalid) ThrowLastWin32Error($"Failed to create service '{definition.Name}'");

        if (definition.StartStopUsers is { Count: > 0 })
        {
            GrantStartStopPermissions(serviceHandle, definition.StartStopUsers);
        }
    }
}