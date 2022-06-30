#if IsWindows
using System.Management;
#endif

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using SmallsOnline.Subnetting.Lib.Models;

namespace DeskHelper.Lib.Models;

/// <summary>
/// Contains info about a network adapter.
/// </summary>
public class NetworkAdapterInfo
{
    public NetworkAdapterInfo(NetworkInterface networkInterface)
    {
        InterfaceId = networkInterface.Id;
        InterfaceName = networkInterface.Name;
        InterfaceMACAddress = ConvertPhysicalAddressToString(networkInterface.GetPhysicalAddress());


        InterfaceIsPhysical = GetIsInterfacePhysical(InterfaceMACAddress);

        IPInterfaceProperties interfaceProperties = networkInterface.GetIPProperties();

        List<Dictionary<string, IPAddress>> ipAddressInfo = GetIPv4AddressesFromProperties(interfaceProperties);
        if (ipAddressInfo.Count is not 0)
        {
            InterfaceIPv4Address = ipAddressInfo[0]["IPAddress"];
            InterfaceSubnetMask = ipAddressInfo[0]["SubnetMask"];
            InterfaceSubnetInfo = new(InterfaceIPv4Address, InterfaceSubnetMask);
        }

        List<IPAddress> gatewayAddresses = GetGatewaysFromProperties(interfaceProperties);
        if (gatewayAddresses.Count is not 0)
        {
            InterfaceIPv4Gateway = gatewayAddresses[0];
        }

        InterfaceDNSServers = GetDNSServersFromProperties(interfaceProperties);

        InterfaceStatus = networkInterface.OperationalStatus;
    }

    /// <summary>
    /// The ID of the network adapter.
    /// </summary>
    public string InterfaceId { get; set; }

    /// <summary>
    /// The name of the network adapter.
    /// </summary>
    public string InterfaceName { get; set; }

    /// <summary>
    /// The MAC address of the network adapter.
    /// </summary>
    public string InterfaceMACAddress { get; set; }

    /// <summary>
    /// Whether the network adapter is a physical adapter or not.
    /// </summary>
    public bool InterfaceIsPhysical { get; set; }

    /// <summary>
    /// The IPv4 address of the network adapter.
    /// </summary>
    public IPAddress? InterfaceIPv4Address { get; set; }

    /// <summary>
    /// The subnet mask of the network adapter.
    /// </summary>
    public IPAddress? InterfaceSubnetMask { get; set; }

    /// <summary>
    /// The IPv4 gateway address of the network adapter.
    /// </summary>
    public IPAddress? InterfaceIPv4Gateway { get; set; }

    /// <summary>
    /// Information about the subnet the IPv4 address is in.
    /// </summary>
    public IPv4Subnet? InterfaceSubnetInfo { get; set; }

    /// <summary>
    /// The DNS servers configured for the network adapter.
    /// </summary>
    public List<IPAddress>? InterfaceDNSServers { get; set; }

    /// <summary>
    /// The current status of the network adapter.
    /// </summary>
    public OperationalStatus InterfaceStatus { get; set; }

    /// <summary>
    /// Whether the network adapter has an IPv4 address or not.
    /// </summary>
    public bool InterfaceHasIPv4Address
    {
        get => InterfaceIPv4Address is not null;
    }

    /// <summary>
    /// Parse the IPv4 addresses from the IP interface properties.
    /// </summary>
    /// <param name="interfaceProperties">Properties of the network adapter</param>
    /// <returns>A collection of dictonary objects with information about the IPv4 addresses.</returns>
    private static List<Dictionary<string, IPAddress>> GetIPv4AddressesFromProperties(IPInterfaceProperties interfaceProperties)
    {
        List<Dictionary<string, IPAddress>> ipAddresses = new();

        foreach (UnicastIPAddressInformation addressInformationItem in interfaceProperties.UnicastAddresses)
        {
            if (addressInformationItem.Address.AddressFamily is AddressFamily.InterNetwork)
            {
                Dictionary<string, IPAddress> ipAddressDict = new()
                {
                    {
                        "IPAddress",
                        addressInformationItem.Address
                    },
                    {
                        "SubnetMask",
                        addressInformationItem.IPv4Mask
                    }
                };

                ipAddresses.Add(ipAddressDict);
            }
        }

        return ipAddresses;
    }

    /// <summary>
    /// Get the IP addresses of the DNS servers for the network adapter.
    /// </summary>
    /// <param name="interfaceProperties">Properties of the network adapter</param>
    /// <returns>A collection of DNS server IP addresses.</returns>
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

    /// <summary>
    /// Get IPv4 gateway addresses for the network adapter.
    /// </summary>
    /// <param name="interfaceProperties">Properties of the network adapter</param>
    /// <returns>A collection of IPv4 addresses.</returns>
    private static List<IPAddress> GetGatewaysFromProperties(IPInterfaceProperties interfaceProperties)
    {
        List<IPAddress> gatewayAddresses = new();

        foreach (GatewayIPAddressInformation gatewayItem in interfaceProperties.GatewayAddresses)
        {
            if (gatewayItem.Address.AddressFamily is AddressFamily.InterNetwork)
            {
                gatewayAddresses.Add(gatewayItem.Address);
            }
        }

        return gatewayAddresses;
    }

    /// <summary>
    /// Convert the physical address (MAC address) to a string.
    /// </summary>
    /// <param name="physicalAddress">A physical address (MAC address)</param>
    /// <returns>A string of the physical/MAC address.</returns>
    private static string ConvertPhysicalAddressToString(PhysicalAddress physicalAddress)
    {
        string interfaceBaseMACAddressString = physicalAddress.ToString();
        int interfaceBaseMACAddressStringLength = interfaceBaseMACAddressString.Length;
        List<string> interfaceMACAddressOctets = new();

        for (int i = 0; i < interfaceBaseMACAddressStringLength; i += 2)
        {
            string octetString = interfaceBaseMACAddressString.Substring(i, Math.Min(2, interfaceBaseMACAddressStringLength - 1));
            interfaceMACAddressOctets.Add(octetString);
        }

        return string.Join(":", interfaceMACAddressOctets);
    }

    /// <summary>
    /// Get whether a network adapter is a physical adapter or not.
    /// </summary>
    /// <param name="macAddress">A string representation of a MAC address.</param>
    /// <returns>Whether a network adapter is physical</returns>
    private static bool GetIsInterfacePhysical(string macAddress)
    {
#if IsWindows
#pragma warning disable CA1416 // Validate platform compatibility
        List<ManagementObject> netAdapters = new();
        using (ManagementObjectSearcher objSearcher = new($"SELECT MACAddress,PhysicalAdapter FROM Win32_NetworkAdapter"))
        {
            foreach (ManagementObject managementObject in objSearcher.Get())
            {
                netAdapters.Add(managementObject);
            }
        }

        bool isPhysical;

            ManagementObject netAdapterProps = netAdapters.Find(
                (ManagementObject item) => (string)item["MACAddress"] == macAddress
            )!;

        try
        {
            if (netAdapterProps is not null)
            {
                isPhysical = (bool)netAdapterProps["PhysicalAdapter"];
            }
            else
            {
                isPhysical = false;
            }
        }
        catch (NullReferenceException)
        {
            isPhysical = false;
        }
#pragma warning restore CA1416 // Validate platform compatibility

        return isPhysical;
#else
        return false;
#endif
    }
}