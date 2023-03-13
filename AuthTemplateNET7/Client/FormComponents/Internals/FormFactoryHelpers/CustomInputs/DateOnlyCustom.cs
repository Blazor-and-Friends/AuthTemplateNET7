using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AuthTemplateNET7.Client.FormComponents.Internals.FormFactoryHelpers.CustomInputs;

//added
public class DateOnlyCustom<TValue> : InputBase<TValue>
{
    const string DATE_FORMAT = "yyyy-MM-dd";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int sequence = 0;

        //this can be refactored into a method on an abstract base class
        builder.OpenElement(sequence++, "input");
        builder.AddMultipleAttributes(sequence++, AdditionalAttributes);
        builder.AddAttribute(sequence++, "type", "date");
        builder.AddAttribute(sequence++, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(sequence++, "class", "form-control " + CssClass);
        builder.AddAttribute(sequence++, "onchange",
            EventCallback.Factory
            .CreateBinder<string>(this,
            value => CurrentValueAsString = value,
            CurrentValueAsString));

        builder.CloseElement();
    }

    protected override string FormatValueAsString(TValue value)
    {
        return value switch
        {
            DateOnly dateOnlyValue => BindConverter.FormatValue(dateOnlyValue, DATE_FORMAT, CultureInfo.InvariantCulture),
            _ => string.Empty
        };
    }

    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage)
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
