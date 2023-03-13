using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Dev;

//added

public class EnvironmentSettingDto
{
    public bool Delete { get; set; }
    public EnvironmentVariableTarget Target { get; set; }
    public string Key { get; set; }
    public bool Update { get; set; }
    public string Value { get; set; }
}
