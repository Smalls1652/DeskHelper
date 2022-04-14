using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;

namespace DeskHelper.Blazor;

public partial class Index : ComponentBase
{
    private ComputerInfo? _computerInfo;

    protected override void OnInitialized()
    {
        _computerInfo = new();
    }
}
