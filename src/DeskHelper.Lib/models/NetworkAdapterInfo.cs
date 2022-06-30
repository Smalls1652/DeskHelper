#if IsWindows
// Available on Windows only.
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
        // Initialize the list of IPv4 addresses.
        List<Dictionary<string, IPAddress>> ipAddresses = new();

        // Loop through each IPv4 address in the 'UnicastAddresses' property.
        foreach (UnicastIPAddressInformation addressInformationItem in interfaceProperties.UnicastAddresses)
        {
            if (addressInformationItem.Address.AddressFamily is AddressFamily.InterNetwork)
            {
                // If the address is IPv4, create a dictonary object with
                // the IPv4 address and it's subnet mask.
                Dictionary<string, IPAddress> ipAddressDict = new()
                {
                    // Add the IPv4 address.
                    {
                        "IPAddress",
                        addressInformationItem.Address
                    },
                    // Add the subnet mask.
                    {
                        "SubnetMask",
                        addressInformationItem.IPv4Mask
                    }
                };

                // Add the dictonary object to the list.
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
        // Initialize the list of DNS servers.
        List<IPAddress> dnsServers = new();

        // Loop through each DNS server in the 'DnsAddresses' property.
        foreach (IPAddress ipAddressItem in interfaceProperties.DnsAddresses)
        {
            if (ipAddressItem.AddressFamily is AddressFamily.InterNetwork)
            {
                // If the address is IPv4, add it to the list.
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
        // Initialize the list of gateway addresses.
        List<IPAddress> gatewayAddresses = new();

        // Loop through each gateway in the 'GatewayAddresses' property.
        foreach (GatewayIPAddressInformation gatewayItem in interfaceProperties.GatewayAddresses)
        {
            if (gatewayItem.Address.AddressFamily is AddressFamily.InterNetwork)
            {
                // If the address is IPv4, add it to the list.
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
        // Convert the physical address to a string.
        string interfaceBaseMACAddressString = physicalAddress.ToString();

        // Get the length of the string.
        int interfaceBaseMACAddressStringLength = interfaceBaseMACAddressString.Length;

        // Initialize a list of strings to hold each octect of the MAC address.
        List<string> interfaceMACAddressOctets = new();

        // Loop through each octect of the MAC address by splitting the string into two characters.
        for (int i = 0; i < interfaceBaseMACAddressStringLength; i += 2)
        {
            // Get the two characters by the substring of the current loop index and the length of MAC address string.
            // Then add octect string to the list.
            string octetString = interfaceBaseMACAddressString.Substring(i, Math.Min(2, interfaceBaseMACAddressStringLength - 1));
            interfaceMACAddressOctets.Add(octetString);
        }

        // Join the octect strings into a single string separated by a colon.
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
        // Initialize the list of physical adapters.
        // Then run a CIM/WMI query on the 'Win32_NetworkAdapter' class.
        List<ManagementObject> netAdapters = new();
        using (ManagementObjectSearcher objSearcher = new($"SELECT MACAddress,PhysicalAdapter FROM Win32_NetworkAdapter"))
        {
            // Loop through each item returned from the query.
            foreach (ManagementObject managementObject in objSearcher.Get())
            {
                // Add the item to the list.
                netAdapters.Add(managementObject);
            }
        }

        bool isPhysical;
        // Find the adapter with the matching MAC address.
        ManagementObject netAdapterProps = netAdapters.Find(
            (ManagementObject item) => (string)item["MACAddress"] == macAddress
        )!;

        try
        {
            if (netAdapterProps is not null)
            {
                // If 'netAdapterProps' is not null,
                // get the value of the 'PhysicalAdapter' property.
                isPhysical = (bool)netAdapterProps["PhysicalAdapter"];
            }
            else
            {
                // Otherwise, set the value to false.
                isPhysical = false;
            }
        }
        catch (NullReferenceException)
        {
            // If a 'NullReferenceException' is thrown, then set the value to false.
            isPhysical = false;
        }
#pragma warning restore CA1416 // Validate platform compatibility

        return isPhysical;
#else
        // Return false if not on Windows.
        // Note: Will need to look into ways to determine this info if on macOS.
        return false;
#endif
    }
}