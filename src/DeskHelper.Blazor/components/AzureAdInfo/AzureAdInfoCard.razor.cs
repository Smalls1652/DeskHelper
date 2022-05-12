using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;

namespace DeskHelper.Blazor.Components.AzureAdInfo;

public partial class AzureAdInfoCard : ComponentBase
{
    [Parameter()]
    [EditorRequired()]
    public AadStatus InputAzureAdInfo { get; set; } = null!;
}
