#if IsWindows
using Microsoft.Win32;
#endif

namespace DeskHelper.Lib.Models;

public class OperatingSystemInfo
{

    public string OSName
    {
        get => _osName;
    }

    public Version OSVersion
    {
        get => _osVersion;
    }

    private readonly string _osName = GetOperatingSystem();

    // Note: If the app is targeting macOS Catalyst, it's not going to report the actual Operating System version number.
    // Need to look into other ways of grabbing this data.
    private readonly Version _osVersion = GetOperatingSystemVersion();
    private static string GetOperatingSystem()
    {
        string osName;

        if (OperatingSystem.IsWindows())
        {
            osName = "Windows";
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            osName = "macOS";
        }
        else
        {
            osName = "Not supported";
        }

        return osName;
    }

    private static Version GetOperatingSystemVersion()
    {
        Version osVersion = Environment.OSVersion.Version;

#if IsWindows
#pragma warning disable CA1416 // Validate platform compatibility
        int osBuildNumber = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", 0)!;
        osVersion = new(osVersion.Major, osVersion.Minor, osVersion.Build, osBuildNumber);
#pragma warning restore CA1416 // Validate platform compatibility
#endif

        return osVersion;
    }
}