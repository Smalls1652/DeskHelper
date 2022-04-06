using System.Net;
using System.Net.NetworkInformation;

namespace DeskHelper.Lib.Models;

public class ComputerInfo
{
    public ComputerInfo()
    {
        _hostName = GetComputerHostName();
        _dnsName = GetComputerDNSName();
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

    public List<NetworkAdapterInfo> NetworkAdapters
    {
        get => _networkAdapters;
    }

    private readonly string _hostName = null!;
    private readonly string _dnsName = null!;
    private readonly List<NetworkAdapterInfo> _networkAdapters = null!;

    private static string GetComputerHostName()
    {
        return Environment.MachineName;
    }

    private static string GetComputerDNSName()
    {
        return Dns.GetHostName();
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