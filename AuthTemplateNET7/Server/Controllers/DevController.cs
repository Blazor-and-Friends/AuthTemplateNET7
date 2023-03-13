using AuthTemplateNET7.Server.Services.EmailingServices;
using AuthTemplateNET7.Server.Services.EmailingServices.NetMailHelpers;
using AuthTemplateNET7.Shared.Dtos;
using AuthTemplateNET7.Shared.Dtos.Dev;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;

namespace AuthTemplateNET7.Server.Controllers;

//added

[Authorize(Roles = "Dev")]
[Route("api/[controller]")]
[ApiController]
public class DevController : ControllerBase
{
    DevRepo devRepo;
    //todo DOCS how i just inject the datacontext rather than doing all the interface, then concrete class, then addscoped. Just more crap for DI to cycle through on each request when it's not even needed. Plus, how often do you switch out your CRUD methods? Or even EF for that matter. The only reason i have the repos is in case i want to make a phone app
    public DevController(DataContext dataContext)
    {
        devRepo = new(dataContext);
    }

    [HttpGet("get-diagnostics")]
    public IActionResult GetDiagnostics([FromServices] BafGlobals bafGlobals)
    {
        var appStartTime = bafGlobals.AppStartTime;
        var utcNow = DateTime.UtcNow;
        var uptimeTimespan = (utcNow - appStartTime);

        DiagnosticsPageModel pageModel = new()
        {
            AppStartTime = bafGlobals.AppStartTime.ToString("HH:mm 'UTC' dd MMM yyyy"),
            Uptime = $"{Math.Floor(uptimeTimespan.TotalHours)}h {uptimeTimespan.Minutes}m {uptimeTimespan.Seconds}s"
        };
        return Ok(pageModel);
    }

    //todo fire and forget at least for emailing: https://www.meziantou.net/fire-and-forget-a-task-in-dotnet.htm

    [HttpGet("get-failed-logins")]
    public async Task<IActionResult> GetFailedLogins()
    {
        return Ok(await devRepo.GetFailedLogins());
    }

    [HttpGet("get-scheduled-controller-guid")]
    public IActionResult GetScheduledControllerGuid([FromServices] IConfiguration configuration)
    {
        var guid = configuration.GetSection("SiteSettings:ScheduledSettings:Guid").Value;
        return Ok(guid);
    }

    #region LogItems

    [HttpPost("delete-all-logitems")]
    public async Task<IActionResult> DeleteAllLogItems()
    {
        var rows = await devRepo.DeleteAllLogItems();

        if(rows >=0) return Ok($"{rows} Log Item(s) deleted");

        return StatusCode(500, "Something went wrong. Refresh the page to see what happened");
    }

    [HttpPost("delete-log-items-by-id")]
    public async Task<IActionResult> DeleteLogItemsById([FromBody] int[] ids)
    {
        var rows = await devRepo.DeleteLogItemsById(ids);

        if (rows >= 0) return Ok($"{rows} Log Item(s) deleted");

        return StatusCode(500, "Something went wrong. Refresh the page to see what happened");
    }

    [HttpGet("get-logitems")]
    public async Task<IActionResult> GetLogItems()
    {
        return Ok(await devRepo.GetLogItems());
    }

    #endregion //LogItems

    #region Settings

    [HttpGet("get-all-environment-variables")]
    public IActionResult GetAllEnvironmentVariables()
    {
        EnvironmentSettingsPageModel model = new();

        var evs = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);
        List<EnvironmentSettingDto> esDtos = new(evs.Count);

        foreach (DictionaryEntry kv in evs)
        {
            esDtos.Add(new() { Key = kv.Key.ToString(), Value = kv.Value.ToString() });
        }
        model.MachineSettings = esDtos.OrderBy(m => m.Key).ToList();

        evs = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User);
        esDtos = new(evs.Count);

        foreach (DictionaryEntry kv in evs)
        {
            esDtos.Add(new() { Key = kv.Key.ToString(), Value = kv.Value.ToString() });
        }
        model.UserSettings = esDtos.OrderBy(m => m.Key).ToList();

        evs = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);
        esDtos= new(evs.Count);

        foreach (DictionaryEntry kv in evs)
        {
            esDtos.Add(new() { Key = kv.Key.ToString(), Value = kv.Value.ToString() });
        }
        model.ProcessSettings = esDtos.OrderBy(m => m.Key).ToList();

        return Ok(model);
    }

    [HttpGet("get-email-settings")]
    public async Task<IActionResult> GetEmailSettings()
    {
        return Ok(await devRepo.GetEmailSettingsAsync());
    }

    [HttpGet("get-settings")]
    public async Task<IActionResult> GetSettings()
    {
        return Ok(await devRepo.GetSettingsAsync());
    }

    [HttpGet("get-net-mail-account")]
    public IActionResult GetEmailSettings([FromServices]BafGlobals bafGlobals)
    {
        var result = new NetMailAccountHelper(bafGlobals).GetDto();
        return Ok(result);
    }

    [HttpPost("set-net-mail-account")]
    public async Task<IActionResult> SetEmailSettings([FromBody] NetMailAccountDto model, [FromServices] BafGlobals bafGlobals, [FromServices]EmailBatchRepo emailBatchRepo)
    {
        var helper = new NetMailAccountHelper(bafGlobals);

        bool valid = false;

        if (model.DeleteAccount)
        {
            helper.DeleteAccount();
            return Ok("Account deleted");
        }

        string msg = null;

        if (ModelState.IsValid)
        {
            valid = true;
            msg = helper.SaveAccount(model);
        }

        if (model.SendSampleEmailTo != null)
        {
            string result = await sendSampleEmail(emailBatchRepo, model.SendSampleEmailTo);

            //todo set the msg based on what the send result is

            if (msg == null) msg = result;
            else msg += " " + result;
        }

        if(!valid && model.SendSampleEmailTo == null) //dev is not deleting account, not sending sample email, so assume s/he's either tried to add or update an account
        {
            return BadRequest("You have invalid fields");
        }

        return Ok(msg);
    }

    [HttpPost("set-environment-variables")]
    public IActionResult SetEnvironmentVariables()
    {
        return StatusCode(500, "Not implemented on the server. Do so at your own risk.");
    }

    [HttpPost("update-email-settings")]
    public async Task<IActionResult> UpdateEmailSettings([FromBody]EmailSettings model)
    {
        if(ModelState.IsValid)
        {
            var success = await devRepo.UpdateEmailSettingsAsync(model);

            if (success) return Ok();

            return StatusCode(500, "Server error, check the log items page");
        }

        return BadRequest("ModelState is not valid");
    }

    [HttpPost("update-settings")]
    public async Task<IActionResult> UpdateSettings([FromBody]DevSettings model)
    {
        if(ModelState.IsValid)
        {
            var success = await devRepo.UpdateSettingsAsync(model);

            if (success) return Ok();

            return StatusCode(500, "Server error. Check your log items page");
        }

        return BadRequest("ModelState is not valid");
    }

    async Task<string> sendSampleEmail(EmailBatchRepo repo, string to)
    {
        var successful = await repo.SendSingleEmailAsync(
            body: $"<h1>Hello Dolly!</h1><p>Sent {DateTime.UtcNow.ToString("HH:mm:ss 'UTC on ' dd MMM yyyy")}</p>",
            subject: "Test email",
            toAddress: to,
            appendUnsubscribeLink: true,
            recipientId: Guid.NewGuid(),
            deleteAfterDays: 2);

        return successful ? $"Email sent to {to}" : "There was a problem sending the test email. Check <a href='dev/log-items'>Log items</a>";
    }

    #endregion //settings
}
