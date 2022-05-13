using System.Text;
using Microsoft.AspNetCore.Components;

namespace DeskHelper.Blazor.Components.UserDetails;

public partial class UserDetailsCard : ComponentBase
{
    [Parameter()]
    [EditorRequired()]
    public string UserName { get; set; } = null!;

    private string GetCopyText()
    {
        StringBuilder stringBuilder = new();
        stringBuilder
            .AppendLine("# 🧑‍💻 User details")
            .AppendLine("")
            .AppendLine("| Name | Value |")
            .AppendLine("| --- | --- |")
            .AppendLine($"| **Currently logged on user** | `{UserName}` |")
            .AppendLine("");

        return stringBuilder.ToString();
    }
}