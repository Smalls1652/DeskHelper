using System.Net.NetworkInformation;
using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace DeskHelper.Blazor.Components.NetworkAdapters;

public partial class NetworkAdaptersList : ComponentBase
{
    [CascadingParameter(Name = "inputAdapters")]
    protected List<NetworkAdapterInfo> InputNetworkAdapters { get; set; } = null!;

    [Inject]
    protected ILogger<NetworkAdaptersList> ComponentLogger { get; set; } = null!;

    [Parameter()]
    public bool OnlyUpNetworkAdapters { get; set; }

    private List<NetworkAdapterInfo>? _networkAdapters;

    protected override void OnParametersSet()
    {
        if (OnlyUpNetworkAdapters is true)
        {
            if (OperatingSystem.IsWindows())
            {
                _networkAdapters = InputNetworkAdapters.FindAll(
                    (NetworkAdapterInfo adapterInfo) => adapterInfo.InterfaceHasIPv4Address is true && adapterInfo.InterfaceIsPhysical is true
                );
            }
            else
            {
                _networkAdapters = InputNetworkAdapters.FindAll(
                    (NetworkAdapterInfo adapterInfo) => adapterInfo.InterfaceHasIPv4Address is true
                );
            }
        }
        else
        {
            if (OperatingSystem.IsWindows())
            {
                _networkAdapters = InputNetworkAdapters.FindAll(
                (NetworkAdapterInfo adapterInfo) => adapterInfo.InterfaceIsPhysical is true
            );
            }
            else
            {
                _networkAdapters = InputNetworkAdapters;
            }
        }
    }
}