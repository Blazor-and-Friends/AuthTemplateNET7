using System.ComponentModel.DataAnnotations;

namespace AuthTemplateNET7.Client.FormComponents;

//added

/// <summary>
/// Determines how checkboxes are displayed. <see href="https://getbootstrap.com/docs/5.0/forms/checks-radios/"/>
/// </summary>
public enum CheckboxPresentationMode
{
    /// <summary>
    /// "form-check"
    /// </summary>
    Stacked,
    /// <summary>
    /// "form-check-inline me-3"
    /// </summary>
    Inline,
    /// <summary>
    /// "form-check form-switch"
    /// </summary>
    Switch,
    /// <summary>
    /// "form-check-inline form-switch me-3"
    /// </summary>
    InlineSwitch
}

/// <summary>
/// Determines how radio inputs are displayed. <see href="https://getbootstrap.com/docs/5.0/forms/checks-radios/"/>
/// </summary>
public enum RadioPresentationMode
{
    /// <summary>
    /// "form-check-inline me-3"
    /// </summary>
    Inline,
    /// <summary>
    /// "form-check"
    /// </summary>
    Stacked
}
