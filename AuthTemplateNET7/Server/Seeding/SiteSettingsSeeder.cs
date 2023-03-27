using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;

namespace AuthTemplateNET7.Server.Seeding;

public class SiteSettingsSeeder
{
    private readonly DataContext dataContext;

    public SiteSettingsSeeder(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public void Seed()
    {
        createAdminSettings();
        createDevSettings();
        createEmailSettings();
    }

    void createAdminSettings()
    {
        AdminSettings adminSettings = new();

        SiteSetting siteSetting = new(AdminSettings.Key, RoleLevel.Admin, adminSettings);
        _ = dataContext.Add(siteSetting);
    }

    void createDevSettings()
    {
        DevSettings devSettings = new();

        SiteSetting siteSetting = new(DevSettings.Key, RoleLevel.Dev, devSettings);
        _ = dataContext.Add(siteSetting);
    }

    void createEmailSettings()
    {
        EmailSettings emailSettings = new EmailSettings()
        {
            EmailingOn = true,
            MaxPerHour = 45,
            MaxPerMinute = 9
        };

        SiteSetting setting = new(EmailSettings.Key, RoleLevel.Dev, emailSettings);
        _ = dataContext.Add(setting);
    }
}
