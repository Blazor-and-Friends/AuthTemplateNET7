using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.Dtos.Admin;
using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTemplateNET7.Server.Controllers;

//added

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    AdminRepo adminRepo;

    public AdminController(DataContext dataContext)
    {
        adminRepo = new AdminRepo(dataContext);
    }

    [HttpGet("get-dashboard-model")]
    public async Task<IActionResult> GetDashboardModel([FromServices]BafGlobals bafGlobals)
    {
        return Ok(await adminRepo.GetDashboardPageModelAsync(bafGlobals.AppName));
    }

    #region contact messages

    [HttpGet("get-contact-messages")]
    public async Task<IActionResult> GetContactMessages()
    {
        return Ok(await adminRepo.GetContactMessagesAsync());
    }

    [HttpPost("update-contact-messages")]
    public async Task<IActionResult> UpdateContactMessages([FromBody] ContactMessage[] arr)
    {
        (int savedCount, int deletedCount, int rows) = await adminRepo.UpdateContactMessages(arr);

        if(rows == -1)
        {
            return StatusCode(500, "There was an issue updating your Contact Messages. This has been logged. Contact your webmaster.");
        }

        return Ok($"{savedCount} saved, {deletedCount} deleted");
    }

    #endregion //contact messages

    #region email batches

    [HttpPost("delete-email-batches-by-ids")]
    public async Task<IActionResult> DeleteEmailBatchesByIds([FromBody] int[] ids)
    {
        var rows = await adminRepo.DeleteEmailBatchesByIds(ids);

        if (rows >= 0) return Ok($"Deleted {rows} Email Batches");
        return StatusCode(500, $"There was a problem deleting {ids.Length} Email Batches. The issue has been logged. Notify your webmaster.");
    }

    [HttpGet("get-email-batches")]
    public async Task<IActionResult> GetEmailBatches()
    {
        //todo at some point prolly should use a dto for EmailBatches so we don't send out the Body for no reason

        return Ok(await adminRepo.GetEmailBatchesAsync());
    }

    [HttpGet("get-emails-for-batch/{id}")]
    public async Task<IActionResult> GetEmailsForBatch(int id)
    {
        return Ok(await adminRepo.GetEmailsForBatch(id));
    }

    [HttpPost("pause-or-resume-batch")]
    public async Task<IActionResult> PauseOrResumeBatch([FromBody]EmailBatch model)
    {
        var success = await adminRepo.PauseOrResumeBatchAsync(model);

        if (success) return Ok();

        if(model.BatchStatus == BatchStatus.Complete)
        {
            return BadRequest("You cannot pause or resume a batch that has been completed.");
        }

        return BadRequest($"There was a problem updating the batch with subject {model.Subject}. The issue has been logged. Contact your webmaster regarding this issue.");
    }

    #endregion //email batches

    #region recipients

    [HttpPost("create-recipient")]
    public async Task<IActionResult> CreateRecipient([FromBody]Recipient model)
    {
        if (ModelState.IsValid)
        {
            (CreateRecipientResult result, Recipient recipient) = await adminRepo.CreateRecipient(model);

            if(result == CreateRecipientResult.Success) return Ok(recipient);

            if(result == CreateRecipientResult.RecipientExists)
            {
                return Conflict(recipient); //409
            }

            if(result == CreateRecipientResult.ServerError) return StatusCode(500, "There was an issue adding the recipient. This has been logged. Contact your webmaster.");
        }

        return BadRequest("You have some invalid fields");
    }

    [HttpGet("get-recipients")]
    public async Task<IActionResult> GetRecipients()
    {
        var result = await adminRepo.GetRecipients();
        return Ok(await adminRepo.GetRecipients());
    }

    [HttpPost("update-recipient")]
    public async Task<IActionResult> UpdateRecipient([FromBody]Recipient model)
    {
        var success = await adminRepo.UpdateRecipient(model);

        if (success) return Ok();

        return StatusCode(500, $"There was an issue updating recipient with address {model.Address}. This issue has been logged. Contact your webmaster.");
    }

    #endregion //recipients

    #region site settings

    [HttpGet("get-settings")]
    public async Task<IActionResult> GetSettings()
    {
        return Ok(await adminRepo.GetSettings());
    }

    [HttpPost("update-settings")]
    public async Task<IActionResult> UpdateSettings([FromBody]AdminSettings model)
    {
        if(ModelState.IsValid)
        {
            var success = await adminRepo.SaveSettingsAsync(model);

            if (success) return Ok();

            return StatusCode(500, "There was a problem updating your site settings. This has been logged. Contact your webmaster.");
        }

        return BadRequest("You have some invalid fields.");
    }

    //[HttpPost("update-site-settings")]
    //public async Task<IActionResult> UpdateSiteSettings([FromBody]AdminSiteSettingsPageModel model)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        var success = await adminRepo.UpdateSiteSettings(model);

    //        if (success) return Ok();

    //        return StatusCode(500, "There was a problem updating your site settings. This has been logged. Contact your webmaster.");
    //    }

    //    return BadRequest("You have invalid fields. If you cannot resolve the issue, contact your webmaster.");
    //}

    #endregion //site settings
}
