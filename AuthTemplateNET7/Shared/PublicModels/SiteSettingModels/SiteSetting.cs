using AuthTemplateNET7.Shared.SharedServices;
using System.ComponentModel.DataAnnotations;

namespace AuthTemplateNET7.Shared.PublicModels.SiteSettingModels;

//added

public class SiteSetting
{
    public int Id { get; set; }

    [Required, MaxLength(32)]
    public string Key { get; set; }

    public RoleLevel RoleLevel { get; set; }

    [Required, MaxLength(2048)]
    public string Value { get; set; }

    [Obsolete("ef, json only", true)]
    public SiteSetting() { }

    public SiteSetting(string key, RoleLevel roleLevel, object obj)
    {
        Key = key;
        RoleLevel = roleLevel;
        Value = obj.ToJson();
    }
}


public enum RoleLevel : byte
{
    Admin,
    Dev
}