using System.Net.NetworkInformation;
using System.Text;
using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;
using SmallsOnline.Subnetting.Lib.Models;

namespace DeskHelper.Blazor.Components.NetworkAdapters;

public partial class NetworkAdapterItem : ComponentBase
{
    [Parameter()]
    public NetworkAdapterInfo AdapterItem { get; set; } = null!;

    private string GetCopyText()
    {
        StringBuilder stringBuilder = new();
        stringBuilder
            .AppendLine($"# 🌐 Network adapter: {AdapterItem.InterfaceName}")
            .AppendLine("")
            .AppendLine("| Name | Value |")
            .AppendLine("| --- | --- |")
#if IsWindows
            .AppendLine($"| **Is adapter up?** | {(AdapterItem.InterfaceStatus is OperationalStatus.Up ? "Yes" : "No")}");
#else
            .AppendLine($"| **Is adapter up?** | {(AdapterItem.InterfaceHasIPv4Address ? "Yes" : "No")}");
#endif

#if IsWindows
        if (AdapterItem.InterfaceStatus is OperationalStatus.Up)
#else
        if (AdapterItem.InterfaceHasIPv4Address is true)
#endif
        {
            stringBuilder.AppendLine($"| **IP address** | `{AdapterItem.InterfaceIPv4Address}` |")
                .AppendLine($"| **Gateway address** | `{AdapterItem.InterfaceIPv4Gateway}` |")
                .AppendLine($"| **DNS servers** | `{(AdapterItem.InterfaceDNSServers!.Count is not 0 ? string.Join(", ", AdapterItem.InterfaceDNSServers!) : "Not set")}` |")
                .AppendLine($"| **Network ID** | `{AdapterItem.InterfaceSubnetInfo}` |")
                .AppendLine($"| **MAC address** | `{AdapterItem.InterfaceMACAddress}` |");
        }

        stringBuilder.AppendLine("");

        return stringBuilder.ToString();
    }
}