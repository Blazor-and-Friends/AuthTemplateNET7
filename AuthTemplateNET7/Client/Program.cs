using AuthTemplateNET7.Client;
using AuthTemplateNET7.Client.ClientServices;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//added
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ClientAuthStateProvider>();
//end added

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<StateManagementService>();

await builder.Build().RunAsync();

#region todo

//todo at some point

// global exception handling on the client

// run site through an accessibility checker

//add auto-expand capablity to TextareaInput.razor, e.g. the textarea height grows as more lines are added

//add some diagnostics to Pages/Dev/DiagnosticsPage.razor

//back to top button is buggy on mobile.

//use sanderson's quickgrid (with Virtualize) as Pages/Dev/LogItemsPage.razor could get to be a long list https://aspnet.github.io/quickgridsamples/getstarted/

//implement a custom validator for Pages/Membership/LoginPage.razor and RegisterPage.razor so validation doesn't get tripped on blur event when the user shows/hides the password field https://shauncurtis.github.io/articles/Blazor-Form-Validation.html


#endregion todo
