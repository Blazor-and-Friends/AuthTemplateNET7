using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.CustomAttributes;

//added

/// <summary>
/// Indicates the IList property from which to populate the HTML option elements for an HTML select element for Client.FormComponents.FormFactory
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class FormFactorySelectSourceAttribute : Attribute
{
    string sourcePropertyName_;

    /// <summary>
    /// Indicates the IList property from which to populate the HTML option elements for an HTML select element for Client.FormComponents.FormFactory
    /// </summary>
    /// <param name="sourcePropertyName">The property name of the List<T> or Array[T]</param>
    /// <exception cref="ArgumentNullException">Need a property name</exception>
    public FormFactorySelectSourceAttribute(string sourcePropertyName)
    {
        sourcePropertyName_ = sourcePropertyName;
    }

    /// <summary>
    /// For internal use
    /// </summary>
    /// <returns>SourcePropertyName</returns>
    public string GetSourcePropertyName()
    {
        return sourcePropertyName_;
    }
}
