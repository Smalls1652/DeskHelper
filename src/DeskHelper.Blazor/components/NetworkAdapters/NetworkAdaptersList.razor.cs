using System.Net.NetworkInformation;
using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;

namespace DeskHelper.Blazor.Components.NetworkAdapters;

public partial class NetworkAdaptersList : ComponentBase
{
    [CascadingParameter(Name = "inputAdapters")]
    protected List<NetworkAdapterInfo> InputNetworkAdapters { get; set; } = null!;

    [Parameter()]
    public bool OnlyUpNetworkAdapters { get; set; }

    private List<NetworkAdapterInfo>? _networkAdapters;

    protected override void OnParametersSet()
    {
        if (OnlyUpNetworkAdapters is true)
        {
#if _WINDOWS
            _networkAdapters = InputNetworkAdapters.FindAll(
                (NetworkAdapterInfo adapterInfo) => adapterInfo.InterfaceStatus is OperationalStatus.Up && adapterInfo.InterfaceIsPhysical is true
            );
#else
            _networkAdapters = InputNetworkAdapters.FindAll(
                (NetworkAdapterInfo adapterInfo) => adapterInfo.InterfaceStatus is OperationalStatus.Up
            );
#endif
        }
        else
        {
#if _WINDOWS
            _networkAdapters = InputNetworkAdapters.FindAll(
                (NetworkAdapterInfo adapterInfo) => adapterInfo.InterfaceIsPhysical is true
            );
#else
            _networkAdapters = InputNetworkAdapters;
#endif
        }
    }
}