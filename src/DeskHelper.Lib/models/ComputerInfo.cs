using System.Management;
using System.Net;
using System.Net.NetworkInformation;

namespace DeskHelper.Lib.Models;

public class ComputerInfo
{
    public ComputerInfo()
    {
        _hostName = GetComputerHostName();
        _dnsName = GetComputerDNSName();
        _computerDomainName = GetComputerDomainName();
        _computerIsDomainJoined = GetIsComputerDomainJoined();
        _networkAdapters = GetNetworkAdapters();
    }

    public string HostName
    {
        get => _hostName;
    }

    public string DNSName
    {
        get => _dnsName;
    }

    public string ComputerDomainName
    {
        get => _computerDomainName;
    }

    public bool ComputerIsDomainJoined
    {
        get => _computerIsDomainJoined;
    }

    public List<NetworkAdapterInfo> NetworkAdapters
    {
        get => _networkAdapters;
    }

    private readonly string _hostName = null!;
    private readonly string _dnsName = null!;
    private readonly string _computerDomainName = null!;
    private readonly bool _computerIsDomainJoined;
    private readonly List<NetworkAdapterInfo> _networkAdapters = null!;

    private static string GetComputerHostName()
    {
        return Environment.MachineName;
    }

    private static string GetComputerDNSName()
    {
        return Dns.GetHostName();
    }

    private static string GetComputerDomainName()
    {
        using ManagementObject computerSystemProps = new(
            path: $"Win32_ComputerSystem.Name='{Environment.MachineName}'"
        );

        return (string)computerSystemProps["Domain"];
    }

    private static bool GetIsComputerDomainJoined()
    {
        using ManagementObject computerSystemProps = new(
            path: $"Win32_ComputerSystem.Name='{Environment.MachineName}'"
        );

        DomainRole domainRole = (DomainRole)computerSystemProps["DomainRole"];

        bool isDomainJoined;
        if (domainRole == DomainRole.MemberWorkstation)
        {
            isDomainJoined = true;
        }
        else
        {
            isDomainJoined = false;
        }

        return isDomainJoined;
    }

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