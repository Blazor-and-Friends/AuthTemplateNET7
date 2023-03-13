using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.CustomAttributes;

//added

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class FormFactoryKeyValueAttribute : Attribute
{
    private string keyName_;
    private string valueName_;

    public FormFactoryKeyValueAttribute(string keyName, string valueName)
    {
        keyName_ = keyName;
        valueName_ = valueName;
    }

    public string GetPropertyKeyName()
    {
        return keyName_;
    }

    public string GetPropertyValueName()
    {
        return valueName_;
    }
}
