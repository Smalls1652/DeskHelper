using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DeskHelper.Blazor.Shared;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    protected NavigationManager NavManager { get; set; } = null!;

    private void HandleRefresh()
    {
        NavManager.NavigateTo(
            uri: "/",
            forceLoad: true
        );
    }
}