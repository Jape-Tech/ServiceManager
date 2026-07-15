using JaPe.ServiceManager.Enums;
using JaPe.ServiceManager.Models;
using Microsoft.Win32;

namespace JaPe.ServiceManager.Native;

public class ServiceRegistryStore
{
    private const string ServiceRegistryPath = @"SYSTEM\CurrentControlSet\Services\";
    public static ServiceDefinition Read(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        
        using var serviceKey = Registry.LocalMachine.OpenSubKey($@"{ServiceRegistryPath}\{name}", writable:false);
        
        if (serviceKey is null)
            throw new InvalidOperationException($"Service '{name}' not found in registry");
        
        var binaryPath = ReadRequiredString(serviceKey, "ImagePath");
        var displayName = ReadRequiredString(serviceKey, "DisplayName");
        var description = ReadOptionalString(serviceKey, "Description");
        var objectName = ReadOptionalString(serviceKey, "ObjectName");
        var start = ReadRequiredDword(serviceKey, "Start");
        var dependencies = ReadMultiString(serviceKey, "DependOnService");
        var environment = ReadEnvironment(serviceKey);

        return new ServiceDefinition
        {
            Name = name,
            BinaryPath = binaryPath,
            DisplayName = displayName,
            Description = description,
            ServiceUser = objectName,
            StartType = MapStartType(start),
            Environment = environment,
        };
    }
    
    public static IReadOnlyDictionary<string,string> ReadEnvironment(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        using var serviceKey = Registry.LocalMachine.OpenSubKey($@"{ServiceRegistryPath}\{name}", writable:false);
        
        if (serviceKey is null)
            throw new InvalidOperationException($"Service '{name}' not found in registry");
        
        return ReadEnvironment(serviceKey);
    }

    private static string ReadRequiredString(RegistryKey key, string valueName)
    {
        var value = key.GetValue(valueName) as string;
        
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Required string value '{valueName}' is missing or empty");

        return value;
    }
    private static string? ReadOptionalString(RegistryKey key, string valueName)
    {
        return key.GetValue(valueName) as string;
    }
    private static uint ReadRequiredDword(RegistryKey key, string valueName)
    {
       var value = key.GetValue(valueName);
       return value switch
       {
           int intValue => unchecked((uint)intValue),
           uint uintValue => uintValue,
           _ => throw new InvalidOperationException($"Invalid value type for '{valueName}'")
       };
    }
    private static IReadOnlyList<string> ReadMultiString(RegistryKey key, string valueName)
    {
        var value = key.GetValue(valueName);

        return value switch
        {
            null => [],
            string[] entries => entries,
            string singleEntry => [singleEntry],
            _ => throw new InvalidOperationException($"Invalid value type for '{valueName}'")
        };
    }
    private static IReadOnlyDictionary<string, string> ReadEnvironment(RegistryKey key)
    {
        var entries = ReadMultiString(key, "Environment");
        var result = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var entry in entries)
        {
            if (string.IsNullOrWhiteSpace(entry))
                continue;
           
            if (!entry.Contains('='))
                continue;
            
            var parts = entry.Split("=", 2);
            result.Add(parts[0], parts[1]);
        }

        return result;
    }
    private static string ToEnvironmentEntry(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        if (key.Contains('='))
            throw new ArgumentException($"Environment variable name cannot contain '='", nameof(key));
        
        return $"{key}={value}";
    }
    private static ServiceStartType MapStartType(uint startType) => startType switch
    {
        0 => ServiceStartType.Boot,
        1 => ServiceStartType.System,
        2 => ServiceStartType.Automatic,
        3 => ServiceStartType.Manual,
        4 => ServiceStartType.Disabled,
        _ => throw new InvalidOperationException($"Invalid start type value '{startType}'")
    };
}