using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Dev;

//added

public class EnvironmentSettingsPageModel
{
    public List<EnvironmentSettingDto> UserSettings { get; set; }

    public List<EnvironmentSettingDto> MachineSettings { get; set; }

    public List<EnvironmentSettingDto> ProcessSettings { get; set; }
}
