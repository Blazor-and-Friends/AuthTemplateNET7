using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using AuthTemplateNET7.Shared.SharedServices;

namespace AuthTemplateNET7.Client.FormComponents.Internals;

//todo Explain how to turn this into a template in README.md

//added
public abstract class BaseInput<T> : InputBase<T>
{
    /// <summary>
    /// Optional. Adds Bootstrap's div.form-text to the component. Default is null.
    /// </summary>
    [Parameter] public RenderFragment FormText { get; set; }

    /// <summary>
    /// Optional. Default is "Enter {DisplayName.ToLower()} here...". Supply an empty string if you do not want a placeholder.
    /// </summary>
    [Parameter] public virtual string Placeholder { get; set; }

    /// <summary>
    /// Optional. If true, adds a red asterisk to the end of the underlying label as an indication to the user that the field is required. Default is false. Note this does nothing for validation, for presentation only.
    /// </summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>
    /// Optional. If set, adds MS's ValidationMessage beneath the input. (Note: Bootstrap called this class "help-text" in version 4 and below.)
    /// </summary>
    [Parameter] public Expression<Func<T>> ValidationFor { get; set; }

    protected string id { get; set; }

    int callCount = 0;

    protected string getClassList(bool isSelectElement = false)
    {
        if (++callCount > 1)
        {
            if (isSelectElement) return "form-select";

            return "form-control";
        }

        string result = isSelectElement ? "form-select" : "form-control";

        if (AdditionalAttributes == null) return result;

        foreach (var item in AdditionalAttributes)
        {
            if(item.Key == "class")
            {
                result += " " + item.Value;
                break;
            }
        }

        return result;
    }

    protected override void OnInitialized()
    {
        id = StringHelpers.GenerateRandomString();
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (Placeholder == null)
        {
            string requiredStr = Required ? "(Required) " : "";
            Placeholder = $"Enter {DisplayName.ToLower()} here...";
        }
        base.OnParametersSet();
    }
}
