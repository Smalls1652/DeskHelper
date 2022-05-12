using Microsoft.AspNetCore.Components;

namespace DeskHelper.Blazor.Components.UserDetails;

public partial class UserDetailsCard : ComponentBase
{
    [Parameter()]
    [EditorRequired()]
    public string UserName { get; set; } = null!;
}