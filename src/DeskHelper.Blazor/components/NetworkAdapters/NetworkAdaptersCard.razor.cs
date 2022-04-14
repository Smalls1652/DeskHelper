using System.Net.NetworkInformation;
using DeskHelper.Lib.Models;
using Microsoft.AspNetCore.Components;

namespace DeskHelper.Blazor.Components.NetworkAdapters;

public partial class NetworkAdaptersCard : ComponentBase
{
    [Parameter()]
    [EditorRequired()]
    public List<NetworkAdapterInfo> InputNetworkAdapters { get; set; } = null!;
    
    private bool _onlyUpNetworkAdapters = true;
    private List<NetworkAdapterInfo>? _networkAdapters;

    protected override void OnParametersSet()
    {
        SetNetworkAdapters();
    }

    private void SetNetworkAdapters()
    {
        if (_onlyUpNetworkAdapters is true)
        {
            _networkAdapters = InputNetworkAdapters.FindAll(
                (NetworkAdapterInfo adapterInfo) => adapterInfo.InterfaceStatus is OperationalStatus.Up
            );
        }
        else
        {
            _networkAdapters = InputNetworkAdapters;
        }

        StateHasChanged();
    }
}