using AuthTemplateNET7.Server.Services.EmailingServices;
using AuthTemplateNET7.Server.Services.EmailingServices.NetMailHelpers;
using AuthTemplateNET7.Server.Services.PaymentServices;
using AuthTemplateNET7.Shared.Dtos;
using AuthTemplateNET7.Shared.Dtos.Dev;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Controllers;

//added

[Authorize(Roles = "Dev")]
[Route("api/[controller]")]
[ApiController]
public class DevController : ControllerBase
{
    DevRepo devRepo;

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

    #region email settings

    [HttpGet("get-email-settings")]
    public async Task<IActionResult> GetEmailSettings()
    {
        return Ok(await devRepo.GetEmailSettingsAsync());
    }

    [HttpPost("update-email-settings")]
    public async Task<IActionResult> UpdateEmailSettings([FromBody] EmailSettings model)
    {
        if (ModelState.IsValid)
        {
            var success = await devRepo.UpdateEmailSettingsAsync(model);

            if (success) return Ok();

            return StatusCode(500, "Server error, check the log items page");
        }

        return BadRequest("ModelState is not valid");
    }

    #endregion //email settings

    #region environment variables

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
        esDtos = new(evs.Count);

        foreach (DictionaryEntry kv in evs)
        {
            esDtos.Add(new() { Key = kv.Key.ToString(), Value = kv.Value.ToString() });
        }
        model.ProcessSettings = esDtos.OrderBy(m => m.Key).ToList();

        return Ok(model);
    }

    [HttpPost("set-environment-variables")]
    public IActionResult SetEnvironmentVariables()
    {
        return StatusCode(500, "Not implemented on the server. Do so at your own risk.");
    }

    #endregion //environment variables

    #region net.mail account

    [HttpGet("check-net-mail-account-status")]
    public IActionResult CheckNetMailAccountStatus([FromServices]BafGlobals bafGlobals)
    {
        return Ok(devRepo.GetNetMailAccountStatusDto(bafGlobals));
    }

    [HttpPost("delete-net-mail-account-info")]
    public IActionResult DeleteNetMailAccountInfo([FromServices]BafGlobals bafGlobals)
    {
        devRepo.DeleteNetMailAccountInfo(bafGlobals);
        return Ok();
    }

    [HttpGet("get-net-mail-account")]
    public async Task<IActionResult> GetNetMailAccount([FromServices] BafGlobals bafGlobals)
    {
        (NetMailAccountDto dto, string errorMessage) = await devRepo.GetNetMailAccountAsync(bafGlobals, getMemberId());

        if(dto != null) return Ok(dto);
        return BadRequest(errorMessage);
    }

    [HttpPost("send-sample-email")]
    public async Task<IActionResult> SendSampleEmail([FromBody]NetMailAccountStatusDto model, [FromServices] EmailBatchRepo emailBatchRepo)
    {
        if(ModelState.IsValid)
        {
            return Ok(await sendSampleEmail(emailBatchRepo, model.SendSampleEmailTo));
        }
        return BadRequest("Not a valid email");
    }


    [HttpPost("set-net-mail-account")]
    public IActionResult SetNetMailAccount([FromBody] NetMailAccountDto model, [FromServices] BafGlobals bafGlobals)
    {
        if (ModelState.IsValid)
        {
            new NetMailAccountHelper(bafGlobals).SaveAccount(model);

            return Ok();
        }

        return BadRequest("You have invalid fields");
    }

    async Task<string> sendSampleEmail(EmailBatchRepo repo, string to)
    {
        var emailSendResult = await repo.SendSingleEmailAsync(
            body: $"<h1><a href='https://www.youtube.com/watch?v=G2MQ_0RUOIA'>Hello Dolly!</a></h1><p>Sent {DateTime.UtcNow.ToString("HH:mm:ss 'UTC on ' dd MMM yyyy")}</p>",
            devOnly: true,
            subject: "Test email",
            toAddress: to,
            appendUnsubscribeLink: true,
            recipientId: Guid.NewGuid(),
            deleteAfterDays: 2);

        switch (emailSendResult)
        {
            case Shared.EmailSendResult.Pending:
                return "The email is pending to be sent";
            case Shared.EmailSendResult.Success:
                return $"Email sent to {to}";
            case Shared.EmailSendResult.Error:
                return "There was a problem sending the test email. Check your <a href='dev/log-items'>Log items</a>";
            default:
                return null;
        }
    }

    #endregion //net.mail account

    #region settings

    [HttpGet("get-settings")]
    public async Task<IActionResult> GetSettings()
    {
        return Ok(await devRepo.GetSettingsAsync());
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


    #endregion //settings

    #region stripe account

    [HttpGet("check-stripe-account-status")]
    public IActionResult CheckStripeAccountStatus([FromServices]BafGlobals bafGlobals)
    {
        return Ok(devRepo.CheckStripeAccountStatus(bafGlobals));
    }

    [HttpGet("get-stripe-account")]
    public async Task<IActionResult> GetStripeAccount([FromServices] BafGlobals bafGlobals)
    {
        var memberId = getMemberId();

        (StripeAccountDto dto, string errorMessage) = await devRepo.GetStripeAccountAsync(bafGlobals, memberId);

        if(dto != null) return Ok(dto);

        return BadRequest(errorMessage);
    }

    [HttpPost("set-stripe-account")]
    public IActionResult SetStripeAccount([FromBody] StripeAccountDto model, [FromServices]BafGlobals bafGlobals)
    {
        devRepo.SetStripeAccount(bafGlobals, model);
        return Ok();
    }

    #endregion //stripe account

    #region helpers

    Guid getMemberId()
    {
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var memberId);
        return memberId;
    }

    #endregion //helpers
}
