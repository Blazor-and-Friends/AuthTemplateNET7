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


await builder.Build().RunAsync();

//todo ASAP a mock emailing service or a workaround for reset password and shit
//todo signature pad: https://github.com/MarvinKlein1508/SignaturePad
//todo payment provider

//a captcha alternative (use the honeypot or time-based) https://www.experienceux.co.uk/ux-blog/5-alternatives-to-captcha-that-wont-baffle-or-frustrate-users/#:~:text=5%20alternatives%20to%20CAPTCHA%20that%20won%E2%80%99t%20baffle%20or,4.%20Simple%20questions%20...%205%205.%20Gamification%20

// global exception handling on the client

//set up a simple implementation of mailjet

//that Kevin Powell semantic css stuff, plus make sure you have all your aria-shit as best you can https://youtu.be/lWu5zf_S9R4?t=96

//add auto-expand capablity to TextareaInput.razor, e.g. the textarea gets bigger as more is added
//add some diagnostics to Pages/Dev/DiagnosticsPage.razor

//back to top button

//implement Pages/Membership/ForgotPasswordPage.razor as well as ResetPasswordPage.razor. Need an email service.

//use sanderson's quickgrid (with Virtualize) as Pages/Dev/LogItemsPage.razor could get to be a long list https://aspnet.github.io/quickgridsamples/getstarted/

//implement recaptcha (at least for the LoginPage and RegisterPage) https://developers.google.com/recaptcha/intro or a simple solution https://stackoverflow.com/a/37841153/2816057

//implement a custom validator for Pages/Membership/LoginPage.razor and RegisterPage.razor so validation doesn't get tripped on blur event when the user shows/hides the password field https://shauncurtis.github.io/articles/Blazor-Form-Validation.html

//change the light/dark theme toggler to a dropdown with the additional option of picking the device default

//run things through an aria- checker as well as an accessibility checker

//todo Docs list
//video for each component or design feature, whatever
//any additional attributes placed on *Input components get added to the underlying input element
//remove the nullable things from .csproj's. Too annoying, and with "required" on properties, no need for it.
//ConfirmDialog has esc key functionality
//explain Components, FormComponents, LayoutComponents, Shared folders
//dataContext.TrySave(string message) and .TrySaveAsync(string message)
//date helpers, string helpers, json helpers
//definitely need to show how the logitems page works. Don't forget title attributes
// Server.Infrastructure.UncaughtExceptionHandler
// HTML sanitizer service
// EditFormWrapper can confirm navigation so user does not lose unsaved changes
// Pbkdf2_HashingService.iterations = 350_000

//todo BEFORE TEMPLATE CREATION
//change the SiteSettings.ScheduledSettings.Guid back to "change-this-string-to-something-unique"
