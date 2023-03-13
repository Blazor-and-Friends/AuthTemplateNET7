using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.ComponentModel.DataAnnotations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using AuthTemplateNET7.Shared.SharedServices;

namespace AuthTemplateNET7.Client.FormComponents.Internals.FormFactoryHelpers.CustomInputs;

//added
public class EnumSelectCustom<TNullableEnum> : InputBase<TNullableEnum>
{
    //hack this isn't working to disable the "Please select..." option. Need to convert this to razor syntax.
    [Parameter] public bool Required { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int sequence = 0;

        builder.OpenElement(sequence++, "select");

        builder.AddMultipleAttributes(sequence++, AdditionalAttributes);
        builder.AddAttribute(sequence++, "value", BindConverter.FormatValue(CurrentValueAsString));
        builder.AddAttribute(sequence++, "class", "form-select " + CssClass);

        builder.AddAttribute(sequence++, "onchange",
            EventCallback.Factory.CreateBinder<string>(
                this,
                value => CurrentValueAsString = value,
                CurrentValueAsString,
                culture: null));


        var isNullable = Nullable.GetUnderlyingType(typeof(TNullableEnum)) != null;

        builder.OpenElement(sequence++, "option");
        if (Required) builder.AddAttribute(sequence++, "disabled", "");
        builder.AddContent(sequence++, "Please select...");
        builder.CloseElement();

        var type = getEnumType();

        foreach (TNullableEnum value in Enum.GetValues(type))
        {
            builder.OpenElement(sequence++, "option");

            builder.AddAttribute(sequence++, "value", value.ToString());
            builder.AddContent(sequence++, getDisplayName(value));

            builder.CloseElement();
        }

        builder.CloseElement();
    }

    Type getEnumType()
    {
        var nullableType = Nullable.GetUnderlyingType(typeof(TNullableEnum));
        if (nullableType != null)
            return nullableType;

        return typeof(TNullableEnum);
    }

    string getDisplayName(TNullableEnum value)
    {
        string text = value.ToString();
        //see if it's got a [DisplayName]
        MemberInfo memberInfo = value.GetType().GetMember(text)[0];
        DisplayAttribute displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

        if (displayAttribute != null) return displayAttribute.GetName();

        return text.TitleCaseToWords();
    }

    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TNullableEnum result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        if (string.IsNullOrEmpty(value))
        {
            result = default(TNullableEnum);
            validationErrorMessage = null;
            return true;
        }

        if (BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out TNullableEnum _result))
        {
            result = _result;
            validationErrorMessage = null;
            return true;
        }

        result = default(TNullableEnum);
        validationErrorMessage = $"The {FieldIdentifier.FieldName} field is not valid.";
        return false;
    }
}
