using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AuthTemplateNET7.Client.ClientServices;

public static class NavigationManagerExtensions
{
    //courtesy of https://chrissainty.com/fragment-routing-with-blazor/
    public static ValueTask NavigateToFragmentAsync(this NavigationManager navigationManager, IJSRuntime jSRuntime)
    {
        Uri uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);

        if (uri.Fragment.Length == 0) return default;

        return jSRuntime.InvokeVoidAsync("tf.scrollToElementById", uri.Fragment.Substring(1));
    }
}
