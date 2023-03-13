using AuthTemplateNET7.Client.ClientServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace AuthTemplateNET7.Client.Bases;

/// <summary>
/// Enables navigating to a section of a page. Adapted from <see href="https://chrissainty.com/fragment-routing-with-blazor/"/>
/// </summary>
public class FragmentNavigationBasePage : ComponentBase, IDisposable
{
    //might as well make these available in case the page needs it
    [Inject] protected NavigationManager Nav { get; set; }
    [Inject] protected IJSRuntime Js { get; set; }

    string currentUrl;

    protected string getAbsoluteUrlWithFragment(string fragment)
    {
        if(!fragment.StartsWith("#")) fragment = "#" + fragment;
        return currentUrl + fragment;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //if(firstRender)
        //{
        //    await Nav.NavigateToFragmentAsync(Js);
        //}
        await Nav.NavigateToFragmentAsync(Js);
    }

    protected override void OnInitialized()
    {
        Nav.LocationChanged += tryFragmentNavigation;
        currentUrl = Nav.Uri.ToString();
    }

    async void tryFragmentNavigation(object sender, LocationChangedEventArgs args)
    {
        await Nav.NavigateToFragmentAsync(Js);
    }

    public void Dispose()
    {
        Nav.LocationChanged -= tryFragmentNavigation;
    }
}
