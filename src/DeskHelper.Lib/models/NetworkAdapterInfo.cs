using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using SmallsOnline.Subnetting.Lib.Models;

namespace DeskHelper.Lib.Models;

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

    public string InterfaceId { get; set; }

    public string InterfaceName { get; set; }

    public string InterfaceMACAddress { get; set; }

    public bool InterfaceIsPhysical { get; set; }

    public IPAddress? InterfaceIPv4Address { get; set; }

    public IPAddress? InterfaceSubnetMask { get; set; }

    public IPAddress? InterfaceIPv4Gateway { get; set; }

    public IPv4Subnet? InterfaceSubnetInfo { get; set; }

    public List<IPAddress>? InterfaceDNSServers { get; set; }

    public OperationalStatus InterfaceStatus { get; set; }

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

    private static bool GetIsInterfacePhysical(string macAddress)
    {
        List<ManagementObject> netAdapters = new();
        using (ManagementObjectSearcher objSearcher = new($"SELECT MACAddress,PhysicalAdapter FROM Win32_NetworkAdapter"))
        {
            foreach (ManagementObject managementObject in objSearcher.Get())
            {
                netAdapters.Add(managementObject);
            }
        }

        bool isPhysical;

        try
        {
            ManagementObject netAdapterProps = netAdapters.Find(
                (ManagementObject item) => (string)item["MACAddress"] == macAddress
            )!;

            isPhysical = (bool)netAdapterProps["PhysicalAdapter"];
        }
        catch
        {
            isPhysical = false;
        }

        return isPhysical;
    }
}