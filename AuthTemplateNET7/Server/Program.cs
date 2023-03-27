using AuthTemplateNET7.Server.Infrastructure;
using AuthTemplateNET7.Server.Services.EmailingServices;
using AuthTemplateNET7.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using AuthTemplateNET7.Server.Services.PaymentServices;

using AuthTemplateNET7.Server.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region added
#if DEBUG
    builder.Services.AddDbContext<DataContext>(opts => opts.UseInMemoryDatabase("dataContext"));
    builder.Services.AddScoped<IEmailService, FakeEmailService>();
#else
    //todo Add production database, IEmailSender
    throw new NotImplementedException("Need a sql database");

    //temp to check release mode
    builder.Services.AddDbContext<DataContext>(opts => opts.UseInMemoryDatabase("dataContext"));
#endif

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Forbidden/";
    options.LoginPath = "/membership/login/";
    options.Cookie.SameSite = SameSiteMode.Strict; //to prevent CSRF
    options.Cookie.HttpOnly = true;
});

builder.Services.AddSingleton<BafGlobals>();

builder.Services.AddScoped<IEmailService, NetMailService>();
builder.Services.AddScoped<StripePaymentService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LinkHelpers>();
builder.Services.AddScoped<EmailBatchRepo>();

builder.Services.AddScoped<IHashPassword, Pbkdf2_HashingService>();

builder.Services.AddControllersWithViews().AddJsonOptions(m => m.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

#endregion //added

//builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();

#region added

BafGlobals bafGlobals = app.Services.GetRequiredService<BafGlobals>(); //for global exception handler and to trigger AppStartTime

#if DEBUG

//seed the data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    string appKey = services.GetRequiredService<BafGlobals>().AppName;

    try
    {
        var dataContext = services.GetRequiredService<DataContext>();

        Seeder seeder = new(dataContext);

        seeder.Seed(appKey);
    }
    catch (Exception e)
    {
        while (e.InnerException != null) e = e.InnerException;
        Console.WriteLine(e.Message);
        throw;
    }
}
#endif


#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    //added Comment out the above --app.UseWebAssemblyDebugging();-- then uncomment this to catch mysterious errors which will then be added to Dev > LogItems
    //app.UseExceptionHandler(configure =>
    //{
    //    configure.Run(async context =>
    //    {
    //        using (var scope = app.Services.CreateScope())
    //        {
    //            var services = scope.ServiceProvider;

    //            var ctx = services.GetRequiredService<DataContext>();
    //            var linkHelpers = services.GetRequiredService<LinkHelpers>();

    //            UncaughtExceptionHandler uncaughtExceptionHandler = new UncaughtExceptionHandler();

    //            await uncaughtExceptionHandler.LogUnhandledException(
    //            context.Features.Get<IExceptionHandlerFeature>().Error,
    //            ctx, bafGlobals, linkHelpers);
    //        }
    //    });
    //});
}
else
{
    //app.UseExceptionHandler("/Error");

    //added
    app.UseExceptionHandler(configure =>
    {
        configure.Run(async context =>
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var ctx = services.GetRequiredService<DataContext>();
                var linkHelpers = services.GetRequiredService<LinkHelpers>();

                UncaughtExceptionHandler uncaughtExceptionHandler = new UncaughtExceptionHandler();

                await uncaughtExceptionHandler.LogUnhandledException(
                context.Features.Get<IExceptionHandlerFeature>().Error,
                ctx, bafGlobals, linkHelpers);

                //need to let the error go back to the client
                //context.Response.Redirect("/Error");
            }
        });
    });

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// added
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
