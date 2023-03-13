using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace AuthTemplateNET7.Client.FormComponents.Internals.FormFactoryHelpers.CustomInputs;

//added
public class NumberInputCustom<TNullableValue> : InputBase<TNullableValue>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int sequence = 0;

        builder.OpenElement(sequence++, "input");

        builder.AddMultipleAttributes(sequence++, AdditionalAttributes);

        builder.AddAttribute(sequence++, "type", "number");
        builder.AddAttribute(sequence++, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(sequence++, "select-all", "");
        builder.AddAttribute(sequence++, "class", "form-control " + CssClass);
        builder.AddAttribute(sequence++, "step", "any");
        builder.AddAttribute(sequence++, "onchange",
            EventCallback.Factory
            .CreateBinder<string>(this,
            value => CurrentValueAsString = value,
            CurrentValueAsString));

        builder.CloseElement();
    }

    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TNullableValue result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        if (BindConverter.TryConvertTo(value, System.Globalization.CultureInfo.InvariantCulture, out result))
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
