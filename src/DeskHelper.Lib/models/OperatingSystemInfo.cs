namespace DeskHelper.Lib.Models;

public class OperatingSystemInfo
{

    public string OSName
    {
        get => GetOperatingSystem();
    }

    public Version OSVersion
    {
        get => _osVersion;
    }

    // Note: If the app is targeting macOS Catalyst, it's not going to report the actual Operating System version number.
    // Need to look into other ways of grabbing this data.
    private readonly Version _osVersion = Environment.OSVersion.Version;
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
}