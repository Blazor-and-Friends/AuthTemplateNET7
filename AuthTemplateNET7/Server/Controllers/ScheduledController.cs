using AuthTemplateNET7.Server.Services.EmailingServices;
using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.Dtos.Membership;
using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Controllers;

//added
/*
 * No need for a repo here as this will never get called from a phone app
 */
[ApiController]
[Route("dev/[controller]")]
public class ScheduledController : ControllerBase
{
    AdminSettings adminSettings { get; set; }
    private readonly DataContext dataContext;
    DevSettings devSettings { get; set; }
    EmailBatchRepo emailBatchRepo;
    string expectedGuid;
#pragma warning disable CS0649
    bool saveChanges;
#pragma warning restore CS0649
    DateTime utcNow = DateTime.UtcNow;

    //todo DOCS a settings table in the db with a setting for turning on/off all aspects of this controller (i.e. the whole thing off, deleting old logitems, old logins. Use that fancy json field in ef 7. This needs to be in a settings table so that the dev can turn off any aspect of the application that is throwing an exception even when s/he's not in front of his/her dev box
    public ScheduledController(EmailBatchRepo emailBatchRepo, IConfiguration configuration, DataContext dataContext)
    {
        this.dataContext = dataContext;
        this.emailBatchRepo = emailBatchRepo;
        expectedGuid = configuration.GetSection("SiteSettings:ScheduledSettings:Guid").Value;
    }

    // dev/scheduled/hourly/guid
    [HttpGet("hourly/{guid?}")]
    public async Task<IActionResult> Hourly(string guid)
    {
        //todo DOCS this in README.md

        if (guid == null || guid != expectedGuid) return BadRequest("Guid must match");

        //todo DOCS change the ScheduledController.Hourly guid and delete this line
        if (guid == "change-this-string-to-something-unique") return BadRequest("Guid not changed in appsettings.json");

        var settings = await dataContext.SiteSettings
            .Where(m => m.Key == AdminSettings.Key || m.Key == DevSettings.Key)
            .ToArrayAsync();

        var settingDev = settings
            .Where(m => m.Key == DevSettings.Key).FirstOrDefault();

        if (settingDev == null) devSettings = new();
        else devSettings = settingDev.Value.FromJson<DevSettings>();

        if (!devSettings.MaintenanceActivityOn) return Ok();

        var settingAdmin = settings
            .Where(m => m.Key == AdminSettings.Key)
            .FirstOrDefault();

        if (settingAdmin == null) adminSettings = new();
        else adminSettings = settingAdmin.Value.FromJson<AdminSettings>();

        if(devSettings.DeleteOldContactMessages) await deleteOldContactMessages(); //once a week Sundays at 3 am utc

        if(devSettings.DeleteOldLogIns) await deleteOldLogins(); //delete any successful once a week at 4 am utc on Sundays

        if(devSettings.DeleteOldLogItems) await deleteOldLogItems(); //once a week on Sundays at 5 am utc

        if(devSettings.DeleteOldEmailBatches) await deleteOldBatches(); //once a week on Mondays at 4 am utc


        if (devSettings.LogMaintenanceActivity && saveChanges) await dataContext.TrySaveAsync("Exception thrown from Server.Controllers.ScheduledController.Hourly()");

        Task.Run(emailBatchRepo.ProcessBatchesAsync).Forget();

        return Ok();
    }

#pragma warning disable CS1998
    async Task deleteOldContactMessages()
    {
        if (utcNow.DayOfWeek == DayOfWeek.Sunday && utcNow.Hour == 3)
        {
            //https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates

            //needs to be wrapped because UseInMemoryDatabase doesn't support relational queries
#if !DEBUG
            var xDaysAgo = utcNow.AddDays(-adminSettings.DeleteContactMessagesOlderThan);
            var rows = await dataContext.ContactMessages.Where(m => m.DateTime < xDaysAgo && !m.SaveMessage).ExecuteDeleteAsync();

            if (devSettings.LogMaintenanceActivity && rows > 0)
            {
                LogItem logItem = new($"Deleted {rows} Contact Message(s)", deleteAfterDays: 8);
                dataContext.Add(logItem);
                saveChanges = true;
            }
#endif
        }
    }

    async Task deleteOldBatches()
    {
        if(utcNow.DayOfWeek == DayOfWeek.Monday && utcNow.Hour == 4)
        {
            //https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates

            //needs to be wrapped because UseInMemoryDatabase doesn't support relational queries
#if !DEBUG
            var rows = await dataContext.EmailBatches.Where(m => m.DeleteAfter < utcNow).ExecuteDeleteAsync();

            if(devSettings.LogMaintenanceActivity && rows > 0)
            {
                LogItem logItem = new($"{rows} email batch(es) deleted", deleteAfterDays: 8);
                dataContext.Add(logItem);
                saveChanges = true;
            }
#endif
        }
    }

    async Task deleteOldLogins()
    {
        if (utcNow.DayOfWeek== DayOfWeek.Sunday && utcNow.Hour == 4)
        {
            //https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates

            //needs to be wrapped because UseInMemoryDatabase doesn't support relational queries
#if !DEBUG
            var xDaysAgo = utcNow.AddDays(-adminSettings.DeleteLoginsOlderThan);
            var rows = await dataContext.Logins.Where(m => m.DateTime < xDaysAgo && m.Success).ExecuteDeleteAsync();

            if(devSettings.LogMaintenanceActivity && rows > 0)
            {
                LogItem logItem = new($"Deleted {rows} login(s)", 8);
                dataContext.Add(logItem);
                saveChanges = true;
            }
#endif
        }
    }

    async Task deleteOldLogItems()
    {
        if (utcNow.DayOfWeek == DayOfWeek.Sunday && utcNow.Hour == 5)
        {
            //https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates

            //needs to be wrapped because UseInMemoryDatabase doesn't support relational queries
#if !DEBUG
            var rows = await dataContext.LogItems.Where(m => m.DeleteAfter < utcNow).ExecuteDeleteAsync();

            if (devSettings.LogMaintenanceActivity && rows > 0)
            {
                LogItem logItem = new($"{rows} log item(s) deleted", 8);
                dataContext.LogItems.Add(logItem);
                saveChanges = true;
            }
#endif
        }
    }
#pragma warning restore CS1998
}
