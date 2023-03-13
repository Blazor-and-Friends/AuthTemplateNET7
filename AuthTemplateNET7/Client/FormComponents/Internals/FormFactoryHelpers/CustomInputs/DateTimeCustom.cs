using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AuthTemplateNET7.Client.FormComponents.Internals.FormFactoryHelpers.CustomInputs;

//added
public class DateTimeCustom<TValue> : InputDate<TValue>
{
    const string DATE_FORMAT = "yyyy-MM-ddTHH:mm";
    Type type = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int sequence = 0;

        builder.OpenElement(sequence++, "input");
        builder.AddMultipleAttributes(sequence++, AdditionalAttributes);
        builder.AddAttribute(sequence++, "type", "datetime-local");
        builder.AddAttribute(sequence++, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(sequence++, "class", "form-control " + CssClass);
        builder.AddAttribute(sequence++, "onchange",
            EventCallback.Factory
            .CreateBinder<string>(this,
            value => CurrentValueAsString = value,
            CurrentValueAsString));

        builder.CloseElement();
    }

    /// <inheritdoc />
    protected override string FormatValueAsString(TValue value)
    {
        return value switch
        {
            DateTime dateTimeValue => BindConverter.FormatValue(dateTimeValue, DATE_FORMAT, CultureInfo.InvariantCulture),
            DateTimeOffset dateTimeOffsetValue => BindConverter.FormatValue(dateTimeOffsetValue, DATE_FORMAT, CultureInfo.InvariantCulture),
            _ => string.Empty
        };
    }

    /// <inheritdoc />
    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        bool successful;
        if (type == typeof(DateTime))
        {
            successful = tryParseDateTime(value, out result);
        }
        else if (type == typeof(DateTimeOffset))
        {
            successful = tryParseDateTimeOffset(value, out result);
        }
        else
        {
            throw new InvalidOperationException($"The type '{type}' is not a supported date type.");
        }

        if (successful)
        {
            validationErrorMessage = null;
            return true;
        }
        else
        {
            validationErrorMessage = string.Format(CultureInfo.CurrentCulture, ParsingErrorMessage, FieldIdentifier.FieldName);
            return false;
        }
    }

    #region helpers

    static bool tryParseDateTime(string value, out TValue result)
    {
        var success = BindConverter.TryConvertToDateTime(value, CultureInfo.InvariantCulture, DATE_FORMAT, out var parsedValue);
        if (success)
        {
            result = (TValue)(object)parsedValue;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    static bool tryParseDateTimeOffset(string value, out TValue result)
    {
        var success = BindConverter.TryConvertToDateTimeOffset(value, CultureInfo.InvariantCulture, DATE_FORMAT, out var parsedValue);
        if (success)
        {
            result = (TValue)(object)parsedValue;
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    #endregion //helpers
}
