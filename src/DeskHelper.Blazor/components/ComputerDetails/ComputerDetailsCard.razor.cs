using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;

namespace DeskHelper.Blazor.Components.ComputerDetails;

public partial class ComputerDetailsCard : ComponentBase
{
    [Parameter()]
    [EditorRequired()]
    public ComputerInfo InputComputerInfo { get; set; } = null!;
}