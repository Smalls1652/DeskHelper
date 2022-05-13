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
            await _copyItemButtonJsModule!.InvokeVoidAsync("copyTextToClipboard", Markdown.ToHtml(
                    markdown: TextToCopy,
                    pipeline: new MarkdownPipelineBuilder()
                        .UsePipeTables()
                        .UseGridTables()
                        .UseSoftlineBreakAsHardlineBreak()
                        .Build()
                )
            );

            _copyInProgress = true;
            StateHasChanged();
            await Task.Delay(2000);

            _copyInProgress = false;
            StateHasChanged();
            await Task.Delay(100);
        }
    }
}