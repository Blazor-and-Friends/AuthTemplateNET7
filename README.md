A .NET 7 Blazor WASM project that is ASP.NET Core hosted with Authentication/Authorization

* Does not rely on nor use any third party Identity providers.
* Includes boiler-plate code such as GDPR cookie consent, Login/Register/Forgot password functionality.
* Includes convenience components to speed up development.

# Setup

* Download or clone the repo, then check out TODO

# Settings

* In `Shared.Dtos.Membership.RegisterDto` set the min and max lengths for the Member's password (currently it is 16/128).

* Auth cookies are set to expire after 60 days in `Server.Program.builder.Services.AddAuthentication().AddCookie()`

* In `Client.ImportsComponents.PageTitleAndHeader.razor`, set the suffix for the `title` element on your website.

* The password hashing service uses Pbkdf2 an the iteration count is set to 350,000 as recommended in [this article](https://www.scottbrady91.com/aspnet-identity/improving-the-aspnet-core-identity-password-hasher). You can change that field in `Server.Services.Pbkdf2_HashingService.iterations`
* In `Server.Data.AuthRepo.RegisterAsyn()`, by default the `Member` is added the "Customer" `Role`.

# Design notes

* In `Server.Program.cs` `builder.Services.AddDbContext<DataContext>(opts => opts.UseInMemoryDatabase("dataContext"));` is wrapped in a `#if DEBUG` compiler directive.

* In `Server.Program.cs` there is a clause which calls `Server.Seeding.Seeder.Seed()` which adds 3 sample members and their respective roles ("Dev", "Admin", "Customer") to the `InMemoryDatabase`.

* The DbContext, `Server.Data.DataContext.cs` has a `TrySaveAsync(string message)` method that wraps `SaveChangesAsync()` in a `try/catch`. If the method fails, it adds a `Server.Models.LogItem` to the database. Note that the `ChangeTracker` is cleared, so don't use it if you plan on massaging your entities to try again.

* Note that roles are case sensitive when used in the authorize attribute, e.g. `[Authorize(Roles = "Admin")]` will not match the role "admin".

* There is a password validation service (`Shared.Services.PasswordValidationService`) which is used in `Shared.Dtos.Membership.RegisterDto.Validate()` as well as in `Server.Controllers.MembershipController.Register`. It is flexible so you can decide whether the password requires a number, upper/lower case letters, and which special characters (if any) to allow. By default all are required.

* There is a file for global using statements at `Server.Infrastructure.GlobalUsings.cs`
* The `Client.ImportsComponents` folder is for anything you want available everywhere in intellisense. `TemplateComponents` and `FormComponents` are not included in `_Imports.razor` by default.

* Anything added to the standard ASP.NET Blazor WASM template has a comment, `//added` if you want to use search to find all the changes. Packages added in the `.csproj` files are marked with added/end added.

# Features

## Added components

These are contained in the `Client/ImportsComponents` folder with is referenced in `_Imports.razor`.

### CookieConsent

* Does what it's supposed to.

### EditFormWrapper

Wraps MS's EditForm and adds some extra features

* Autofocus first `input/textarea/select` in the form. This is not supported in Firefox (Jan 2023). Regardless, if you forget to add it to the first `input`, the `EditFormWrapper` will automatically focus the first `input`.

* When the form is invalidly submitted, the form is scrolled to the top where a `ValidationSummary` is displayed.

* Adds a `novalidate` attribute to the form to short-circuit browser validation.
* Automatically adds the `SubmitButton`.
* Add a `select-all` attribute on any `input`, `textarea` elements or Blazor equivalents and all text will be selected when the input gains focus. It's good for `InputNumber` which will have the default value of 0 in it, or for inputs where it's likely the user would paste content into that field. *Note:* This works outside of the `EditFormWrapper` as well

### Footer

* Stays at the bottom of the viewport when there isn't enough content to fill the page.
* Dynamically resizes itself according to its content. No need to worry about the old-time way of setting a fixed height on it.

### LoginPartial
* Has `returnUrl` functionality to return to the page from which the user clicked login/register.

### Navbar

Replaces the `NavMenu.razor` component that comes with MS's ASP.NET Core Blazor WASM template with Bootstrap's navbar

* Has a light/dark theme toggler. The light theme is Bootstrap's theme, the dark theme is Bootswatch's darkly theme.

### PageTitleAndHeader

Sets the `title` element and `h1.text-centered` header for your page. Optionally automatically adds a suffix to the `title` element, e.g. "Home" becomes "Home - MyWebsite.com".

## Form components

### PasswordInput
* Includes a show/hide button.
* Automatically refocuses the input so that smartphones don't drop their keyboard.

### SubmitButton

* You need to add a ref attribute to it, then in your `OnValidSubmit` method, call `SubmitButton.ToggleDisabled()` before you process your form.
* Adds a spinner to the button to indicate to the user something's happening.
* This is automatically included in the `EditFormWrapper`.


# Issues/Todos

* In the `Client.Pages.Membership.RegisterPage` and `Client.Pages.Membership` pages, there's an issue where validation gets triggered when a user clicks on the Show/Hide password. You can implement a custom validator that only triggers on form submit to bypass this issue. [See this S.O. answer](https://stackoverflow.com/a/70426520/2816057).