using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace DeskHelper.Blazor.Components.AzureAdInfo;

public partial class AzureAdInfoCard : ComponentBase
{
    [Parameter()]
    [EditorRequired()]
    public AadStatus InputAzureAdInfo { get; set; } = null!;

    private string GetCopyText()
    {
        StringBuilder stringBuilder = new();
        stringBuilder
            .AppendLine("# ☁️ Azure AD details")
            .AppendLine("")
            .AppendLine("| Name | Value |")
            .AppendLine("| --- | --- |")
            .AppendLine($"| **Device ID** | `{InputAzureAdInfo.DeviceId}` |")
            .AppendLine($"| **Tenant ID** | `{InputAzureAdInfo.TenantId}` |")
            .AppendLine($"| **Is Azure AD joined?** | {(InputAzureAdInfo.AzureAdJoined ? "Yes" : "No")} |")
            .AppendLine($"| **Is Enterprise joined?** | {(InputAzureAdInfo.EnterpriseJoined ? "Yes" : "No")} |")
            .AppendLine($"| **Is AD domain joined?** | {(InputAzureAdInfo.DomainJoined ? "Yes" : "No")} |")
            .AppendLine("");

        return stringBuilder.ToString();
    }
}
