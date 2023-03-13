using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AuthTemplateNET7.Client.FormComponents.Internals;

//added
public abstract class DateBase<TNullable> : BaseInput<TNullable>
{
    protected Type type;

    protected override void OnInitialized()
    {
        type = Nullable.GetUnderlyingType(typeof(TNullable)) ?? typeof(TNullable);
    }

    protected override bool TryParseValueFromString(string value, out TNullable result, out string validationErrorMessage)
    {
        if (BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out result))
        {
            validationErrorMessage = null;
            return true;
        }
        else
        {
            string name = DisplayName ?? FieldIdentifier.FieldName;
            validationErrorMessage = $"Could not parse {name}";
            return false;
        }
    }
}
