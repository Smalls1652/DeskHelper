using Microsoft.AspNetCore.Components;

namespace DeskHelper.Blazor.Components.Core;

public partial class MonospaceText : ComponentBase
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}