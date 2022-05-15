using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DeskHelper.Blazor.Components.Core;

public partial class CopyItemButton : ComponentBase
{
    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public string TextToCopy { get; set; } = null!;

    [Parameter]
    public string ButtonPrimaryColorClass { get; set; } = "btn-light";

    [Parameter]
    public string ButtonOnCopyColorClass { get; set; } = "btn-lightgreen";

    private bool _copyInProgress = false;

    private IJSObjectReference? _copyItemButtonJsModule;

    protected override async Task OnInitializedAsync()
    {
        _copyItemButtonJsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/DeskHelper.Blazor/components/core/CopyItemButton.razor.js");

        await base.OnInitializedAsync();
    }

    private async void CopyTextToClipboard()
    {
        if (_copyInProgress is false)
        {
            string convertedText = Markdown.ToHtml(
                markdown: TextToCopy,
                pipeline: new MarkdownPipelineBuilder()
                    .UsePipeTables()
                    .Build()
            );

            await _copyItemButtonJsModule!.InvokeVoidAsync("copyTextToClipboard", convertedText);

            _copyInProgress = true;
            StateHasChanged();
            await Task.Delay(2000);

            _copyInProgress = false;
            StateHasChanged();
            await Task.Delay(100);
        }
    }
}