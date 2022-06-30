#if IsWindows
// Available on Windows only.
using Microsoft.Win32;
#endif

namespace DeskHelper.Lib.Models;

/// <summary>
/// Contains info about the computer's operating system.
/// </summary>
public class OperatingSystemInfo
{
    /// <summary>
    /// The name of the operating system.
    /// </summary>
    public string OSName
    {
        get => _osName;
    }

    /// <summary>
    /// The currently reported version of the operating system.
    /// </summary>
    public Version OSVersion
    {
        get => _osVersion;
    }

    private readonly string _osName = GetOperatingSystem();

    // Note: If the app is targeting macOS Catalyst, it's not going to report the actual Operating System version number.
    // Need to look into other ways of grabbing this data.
    private readonly Version _osVersion = GetOperatingSystemVersion();

    /// <summary>
    /// Get the computer's operating system.
    /// </summary>
    /// <returns>The operating system name.</returns>
    private static string GetOperatingSystem()
    {
        string osName;

        if (OperatingSystem.IsWindows())
        {
            // If the method 'IsWindows()' returns true,
            // then set the OS name to 'Windows'.
            osName = "Windows";
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            // If the methods 'IsMacOS()' or 'IsMacCatalyst()' return true,
            // then set the OS name to 'macOS'.
            osName = "macOS";
        }
        else
        {
            // Otherwise, set the OS name to 'Not supported'.
            osName = "Not supported";
        }

        return osName;
    }

    /// <summary>
    /// Get the current version of the operating system.
    /// </summary>
    /// <returns>The version of the operating system.</returns>
    private static Version GetOperatingSystemVersion()
    {
        // Get the OS version from the 'OSVersion' environment variable.
        Version osVersion = Environment.OSVersion.Version;

#if IsWindows
// Available on Windows only.
#pragma warning disable CA1416 // Validate platform compatibility
        // Get the current build number from the 'UBR' property in the 'HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion' registry key.
        // Then update the OS version to include the build number as well.
        int osBuildNumber = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", 0)!;
        osVersion = new(osVersion.Major, osVersion.Minor, osVersion.Build, osBuildNumber);
#pragma warning restore CA1416 // Validate platform compatibility
#endif

        return osVersion;
    }
}