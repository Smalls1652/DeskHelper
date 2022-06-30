#if IsWindows
using System.Management;
#endif

using System.Net;
using System.Net.NetworkInformation;

namespace DeskHelper.Lib.Models;

/// <summary>
/// Contains data about a computer.
/// </summary>
public class ComputerInfo
{
    public ComputerInfo()
    {
        _hostName = GetComputerHostName();
        _dnsName = GetComputerDNSName();

#if IsWindows
        _computerDomainName = GetComputerDomainName();
        _computerIsDomainJoined = GetIsComputerDomainJoined();
        _azureAdInfo = new();
#else
        _computerDomainName = "Not supported";
        _computerIsDomainJoined = false;
#endif

        _networkAdapters = GetNetworkAdapters();
    }

    /// <summary>
    /// The hostname of the computer.
    /// </summary>
    public string HostName
    {
        get => _hostName;
    }

    /// <summary>
    /// The DNS name of the computer.
    /// </summary>
    public string DNSName
    {
        get => _dnsName;
    }

    /// <summary>
    /// The domain name the computer is joined to.
    /// </summary>
    public string? ComputerDomainName
    {
        get => _computerDomainName;
    }

    /// <summary>
    /// Whether the computer is joined to an AD domain.
    /// </summary>
    public bool ComputerIsDomainJoined
    {
        get => _computerIsDomainJoined;
    }

    /// <summary>
    /// The current user. Specifically the current user executing the program.
    /// </summary>
    public string CurrentUser
    {
        get => Environment.UserName;
    }

    /// <summary>
    /// The computer's Azure AD info collected through the dsregcmd CLI tool.
    /// </summary>
    public AadStatus? AzureAdInfo
    {
        get => _azureAdInfo;
    }

    /// <summary>
    /// The computer's operating system info.
    /// </summary>
    public OperatingSystemInfo OSInfo
    {
        get => _osInfo;
    }

    /// <summary>
    /// A list of network adapters/interfaces on the computer.
    /// </summary>
    public List<NetworkAdapterInfo> NetworkAdapters
    {
        get => _networkAdapters;
    }

    private readonly string _hostName = null!;
    private readonly string _dnsName = null!;
    private readonly string? _computerDomainName;
    private readonly bool _computerIsDomainJoined;
    private AadStatus? _azureAdInfo;
    private readonly OperatingSystemInfo _osInfo = new();
    private readonly List<NetworkAdapterInfo> _networkAdapters = null!;

    /// <summary>
    /// Get the computer's hostname.
    /// </summary>
    /// <returns>The hostname of the computer.</returns>
    private static string GetComputerHostName()
    {
        return Environment.MachineName;
    }

    /// <summary>
    /// Get the computer's DNS name.
    /// </summary>
    /// <returns>The DNS name of the computer.</returns>
    private static string GetComputerDNSName()
    {
        return Dns.GetHostName();
    }

#if IsWindows
#pragma warning disable CA1416 // Validate platform compatibility

    /// <summary>
    /// Get the AD domain name the computer is joined to.
    /// </summary>
    /// <returns>The domain name the computer is joined to.</returns>
    private static string GetComputerDomainName()
    {
        using ManagementObject computerSystemProps = new(
            path: $"Win32_ComputerSystem.Name='{Environment.MachineName}'"
        );

        return (string)computerSystemProps["Domain"];
    }

    /// <summary>
    /// Get whether the computer is joined to an AD domain.
    /// </summary>
    /// <returns>Whether the computer is joined to an AD domain.</returns>
    private static bool GetIsComputerDomainJoined()
    {
        using ManagementObject computerSystemProps = new(
            path: $"Win32_ComputerSystem.Name='{Environment.MachineName}'"
        );

        DomainRole domainRole = (DomainRole)computerSystemProps["DomainRole"];

        bool isDomainJoined;
        if (domainRole == DomainRole.MemberWorkstation || domainRole == DomainRole.MemberServer)
        {
            isDomainJoined = true;
        }
        else
        {
            isDomainJoined = false;
        }

        return isDomainJoined;
    }
#pragma warning restore CA1416 // Validate platform compatibility
#endif

    /// <summary>
    /// Get a list of network adapters/interfaces on the computer.
    /// </summary>
    /// <returns>A collection of network adapters.</returns>
    private static List<NetworkAdapterInfo> GetNetworkAdapters()
    {
        List<NetworkAdapterInfo> networkAdapters = new();

        List<NetworkInterface> networkInterfaces = new(
            NetworkInterface.GetAllNetworkInterfaces()
        );

        List<NetworkInterface> filteredNetworkInterfaces = networkInterfaces.FindAll(
            (NetworkInterface item) => item.NetworkInterfaceType is NetworkInterfaceType.Ethernet || item.NetworkInterfaceType is NetworkInterfaceType.Wireless80211
        );

        foreach (NetworkInterface interfaceItem in filteredNetworkInterfaces)
        {
            networkAdapters.Add(
                new(interfaceItem)
            );
        }

        networkAdapters.Sort(
            (NetworkAdapterInfo item1, NetworkAdapterInfo item2) => item1.InterfaceName.CompareTo(item2.InterfaceName)
        );

        return networkAdapters;
    }
}