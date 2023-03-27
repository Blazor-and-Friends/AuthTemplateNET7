using Microsoft.AspNetCore.Components;

namespace AuthTemplateNET7.Client.FormComponents.Internals;

//added
public abstract class SelectBase<T> : BaseInput<T>
{
    /// <summary>
    /// Optional. The text that will be displayed in the first option element. Default is "Please select..."
    /// </summary>
    [Parameter] public override string Placeholder { get; set; }

    protected bool firstOptionSelected;
    protected string firstOptionValue;
    protected Type type;

    protected override void OnInitialized()
    {
        type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Placeholder == null)
        {
            string requiredStr = Required ? "(Required) " : "";
            Placeholder = $"{requiredStr}Please select...";
        }
    }
}
