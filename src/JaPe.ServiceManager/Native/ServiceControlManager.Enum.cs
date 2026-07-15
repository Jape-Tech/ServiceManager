using Windows.Win32;
using Windows.Win32.System.Services;
using System.Runtime.InteropServices;
using JaPe.ServiceManager.Models;

namespace JaPe.ServiceManager.Native;
internal sealed partial class ServiceControlManager
{
    public  List<ServiceEnumItem> EnumServices()
    {
        uint bytesNeeded = 0;
        uint servicesReturned = 0;
        uint resumeHandle = 0;

        PInvoke.EnumServicesStatusEx(
            hSCManager: _scmHandle,
            InfoLevel: SC_ENUM_TYPE.SC_ENUM_PROCESS_INFO,
            dwServiceType: ENUM_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS,
            dwServiceState: ENUM_SERVICE_STATE.SERVICE_STATE_ALL,
            pcbBytesNeeded: out bytesNeeded,
            lpServicesReturned: out servicesReturned,
            lpResumeHandle: ref resumeHandle
        );
        
        
        if (bytesNeeded == 0) ThrowLastWin32Error("Failed to get buffer size for service enumeration");
        
        var buffer = new byte[bytesNeeded];
        var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        resumeHandle = 0;
        try
        {
                var success = PInvoke.EnumServicesStatusEx(
                hSCManager: _scmHandle,
                InfoLevel: SC_ENUM_TYPE.SC_ENUM_PROCESS_INFO,
                dwServiceType: ENUM_SERVICE_TYPE.SERVICE_WIN32_OWN_PROCESS,
                dwServiceState: ENUM_SERVICE_STATE.SERVICE_STATE_ALL,
                lpServices: buffer,
                pcbBytesNeeded: out bytesNeeded,
                lpServicesReturned: out servicesReturned,
                lpResumeHandle: ref resumeHandle
            );
            if (!success) ThrowLastWin32Error("Failed to enumerate services");
            // print buffer content for debug
            // Console.WriteLine(Encoding.Unicode.GetString(buffer));
            var services = new List<ServiceEnumItem>((int)servicesReturned);
            var structureSize = Marshal.SizeOf<ENUM_SERVICE_STATUS_PROCESSW>();
            var bufferAddress = pinnedBuffer.AddrOfPinnedObject();
            
            for (var index = 0; index < servicesReturned; index++)
            {
                var structureAddress = IntPtr.Add(bufferAddress, checked((int)index * structureSize));
                var service = Marshal.PtrToStructure<ENUM_SERVICE_STATUS_PROCESSW>(structureAddress);
                services.Add(new ServiceEnumItem
                {
                    Name = service.lpServiceName.ToString(),
                    DisplayName = service.lpDisplayName.ToString(),
                });
            }
            
            return services;
        }
        finally
        {
            pinnedBuffer.Free();
        }
    }
}