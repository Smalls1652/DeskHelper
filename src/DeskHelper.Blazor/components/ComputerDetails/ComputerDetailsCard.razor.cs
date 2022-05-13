using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace DeskHelper.Blazor.Components.ComputerDetails;

public partial class ComputerDetailsCard : ComponentBase
{
    [Parameter()]
    [EditorRequired()]
    public ComputerInfo InputComputerInfo { get; set; } = null!;

    private string GetCopyText()
    {
        StringBuilder stringBuilder = new();
        stringBuilder
            .AppendLine("# 💻 Computer details")
            .AppendLine("")
            .AppendLine("| Name | Value |")
            .AppendLine("| --- | --- |")
            .AppendLine($"| **Computer name** | `{InputComputerInfo.HostName}` |")
            .AppendLine($"| **Domain name** | `{InputComputerInfo.ComputerDomainName}` |")
            .AppendLine($"| **Domain joined** | `{InputComputerInfo.ComputerIsDomainJoined}` |")
            .AppendLine($"| **Operating System** | `{InputComputerInfo.OSInfo.OSName}` |")
            .AppendLine($"| **Operating System version** | `{InputComputerInfo.OSInfo.OSVersion}` |")
            .Append("\n\n");

        return stringBuilder.ToString();
    }
}