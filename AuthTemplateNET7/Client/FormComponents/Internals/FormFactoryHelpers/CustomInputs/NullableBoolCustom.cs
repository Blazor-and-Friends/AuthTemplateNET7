using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace AuthTemplateNET7.Client.FormComponents.Internals.FormFactoryHelpers.CustomInputs;

//added
public class NullableBoolCustom : InputBase<bool?>
{

    [DisallowNull] public ElementReference? Element { get; protected set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        int sequence = 0;
        builder.OpenElement(sequence++, "input");

        builder.AddMultipleAttributes(sequence++, AdditionalAttributes);
        builder.AddAttribute(sequence++, "type", "checkbox");
        builder.AddAttribute(sequence++, "class", "form-check-input " + CssClass);
        builder.AddAttribute(sequence++, "checked", BindConverter.FormatValue(CurrentValue));
        builder.AddAttribute(sequence++, "onchange",
            EventCallback.Factory.CreateBinder<bool?>(this,
            __value => CurrentValue = __value, CurrentValue));
        builder.SetUpdatesAttributeName("checked");
        builder.AddElementReferenceCapture(sequence++,
            __inputReference => Element= __inputReference);

        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out bool? result, [NotNullWhen(false)] out string validationErrorMessage)
    {
        throw new NotSupportedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");
    }
}
