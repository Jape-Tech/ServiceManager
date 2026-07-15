using System.Security.AccessControl;
using System.Security.Principal;
using Windows.Win32;
using Windows.Win32.Security;
using Windows.Win32.System.Services;
using JaPe.ServiceManager.Enums;

namespace JaPe.ServiceManager.Native;

internal sealed partial class ServiceControlManager
{
    private static void GrantStartStopPermissions(
        CloseServiceHandleSafeHandle serviceHandle,
        IEnumerable<string> users)
    {
       var securityDescriptor = GetServiceSecurityDescriptor(serviceHandle);
       var discretionaryAcl = securityDescriptor.DiscretionaryAcl;
       
       if (discretionaryAcl == null)
       {
           throw new InvalidOperationException("Discretionary ACL is null");
       }

       foreach (var user in users)
       {
           var sid = ResolveSid(user);
           
           discretionaryAcl.AddAccess(
               AccessControlType.Allow,
               sid,
               NativeConstants.ServiceStartStopAccess,
               InheritanceFlags.None,
               PropagationFlags.None
               );
       }
       
       SetServiceSecurityDescriptor(serviceHandle, securityDescriptor);

    }

    public IReadOnlyList<string> GetStartStopUsers(string serviceName)
    {
        ArgumentException.ThrowIfNullOrEmpty(serviceName);

        using var serviceHandle = PInvoke.OpenService(
            hSCManager: _scmHandle,
            lpServiceName: serviceName,
            dwDesiredAccess: NativeConstants.ReadControl
        );
        
        if (serviceHandle.IsInvalid)
            ThrowLastWin32Error($"Failed to open service {serviceName}");
        
        return GetStartStopUsers(serviceHandle);
    }
    private IReadOnlyList<string> GetStartStopUsers(CloseServiceHandleSafeHandle serviceHandle)
    {
        var securityDescriptor = GetServiceSecurityDescriptor(serviceHandle);
        var discretionaryAcl = securityDescriptor.DiscretionaryAcl;

        if (discretionaryAcl == null)
        {
            return [];
        }

        var users = new List<string>();

        foreach (CommonAce ace in discretionaryAcl)
        {
            if (ace.AceType != AceType.AccessAllowed)
                continue;
            
            var hasStartStopPermission = 
                (ace.AccessMask & NativeConstants.ServiceStart) == NativeConstants.ServiceStart &&
                (ace.AccessMask & NativeConstants.ServiceStop) == NativeConstants.ServiceStop;
            
            if (!hasStartStopPermission)
                continue;
            
            users.Add(TranslateSidToAccountName(ace.SecurityIdentifier));
        }

        return users
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string TranslateSidToAccountName(SecurityIdentifier sid)
    {
        try
        {
            return sid.Translate(typeof(NTAccount)).Value;
        }
        catch (IdentityNotMappedException)
        {
            return sid.Value;
        }
    }

    private static SecurityIdentifier ResolveSid(string accountName)
    {
        ArgumentException.ThrowIfNullOrEmpty(accountName);
        
        return (SecurityIdentifier)new NTAccount(accountName)
            .Translate(typeof(SecurityIdentifier));
    }
    
    private static unsafe CommonSecurityDescriptor GetServiceSecurityDescriptor(
        CloseServiceHandleSafeHandle serviceHandle)
    {
        uint bytesNeeded = 0;
        var success = false;

        PInvoke.QueryServiceObjectSecurity(
            hService: serviceHandle,
            dwSecurityInformation: (uint)OBJECT_SECURITY_INFORMATION.DACL_SECURITY_INFORMATION,
            cbBufSize: 0,
            pcbBytesNeeded: out bytesNeeded
            );

        if (bytesNeeded == 0)
        {
            ThrowLastWin32Error("Failed to query service security descriptor size");
        }
        
        var buffer = new byte[bytesNeeded];

        fixed (byte* lpSecurityDescriptor = buffer)
        {
            success = PInvoke.QueryServiceObjectSecurity(
                hService: serviceHandle,
                dwSecurityInformation: (uint)OBJECT_SECURITY_INFORMATION.DACL_SECURITY_INFORMATION,
                lpSecurityDescriptor: new PSECURITY_DESCRIPTOR(lpSecurityDescriptor),
                cbBufSize: bytesNeeded,
                pcbBytesNeeded: out _
            );
        }

        if (!success)
        {
            ThrowLastWin32Error("Failed to query service security descriptor");
        }
        
        return new CommonSecurityDescriptor(
            isContainer: false,
            isDS: false,
            binaryForm: buffer,
            offset: 0
            );
    }

    private static unsafe void SetServiceSecurityDescriptor(
        CloseServiceHandleSafeHandle serviceHandle,
        CommonSecurityDescriptor securityDescriptor)
    {
        var buffer = new byte[securityDescriptor.BinaryLength];
        securityDescriptor.GetBinaryForm(buffer, 0);

        fixed (byte* bufferPointer = buffer)
        {
            var success = PInvoke.SetServiceObjectSecurity(
                hService: serviceHandle,
                dwSecurityInformation: OBJECT_SECURITY_INFORMATION.DACL_SECURITY_INFORMATION,
                lpSecurityDescriptor: new PSECURITY_DESCRIPTOR(bufferPointer)
            );
            if (!success)
            {
                ThrowLastWin32Error("Failed to set service security descriptor");
            }
        }
    }
}