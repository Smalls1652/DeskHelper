using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;
using SmallsOnline.Subnetting.Lib.Models;

namespace DeskHelper.Blazor.Components.NetworkAdapters;

public partial class NetworkAdapterItem : ComponentBase
{
    [Parameter()]
    public NetworkAdapterInfo AdapterItem { get; set; } = null!;
}