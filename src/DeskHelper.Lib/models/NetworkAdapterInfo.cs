using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DeskHelper.Lib.Models;

public class NetworkAdapterInfo
{
    public NetworkAdapterInfo(NetworkInterface networkInterface)
    {
        InterfaceId = networkInterface.Id;
        InterfaceName = networkInterface.Name;
        InterfaceMACAddress = networkInterface.GetPhysicalAddress();

        IPInterfaceProperties interfaceProperties = networkInterface.GetIPProperties();
        InterfaceIPv4Addresses = GetIPv4AddressesFromProperties(interfaceProperties);
        InterfaceDNSServers = GetDNSServersFromProperties(interfaceProperties);

        InterfaceStatus = networkInterface.OperationalStatus;
    }

    public string InterfaceId { get; set; }

    public string InterfaceName { get; set; }

    public PhysicalAddress InterfaceMACAddress { get; set; }

    public List<IPAddress> InterfaceIPv4Addresses { get; set; }

    public List<IPAddress> InterfaceDNSServers { get; set; }

    public OperationalStatus InterfaceStatus { get; set; }

    private static List<IPAddress> GetIPv4AddressesFromProperties(IPInterfaceProperties interfaceProperties)
    {
        List<IPAddress> ipAddresses = new();

        foreach (UnicastIPAddressInformation addressInformationItem in interfaceProperties.UnicastAddresses)
        {
            if (addressInformationItem.Address.AddressFamily is AddressFamily.InterNetwork)
            {
                ipAddresses.Add(addressInformationItem.Address);
            }
        }

        return ipAddresses;
    }

    private static List<IPAddress> GetDNSServersFromProperties(IPInterfaceProperties interfaceProperties)
    {
        List<IPAddress> dnsServers = new();
        
        foreach (IPAddress ipAddressItem in interfaceProperties.DnsAddresses)
        {
            if (ipAddressItem.AddressFamily is AddressFamily.InterNetwork)
            {
                dnsServers.Add(ipAddressItem);
            }
        }

        return dnsServers;
    }
}