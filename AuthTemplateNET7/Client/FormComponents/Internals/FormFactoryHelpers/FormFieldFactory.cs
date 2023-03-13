using AuthTemplateNET7.Shared.CustomAttributes;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AuthTemplateNET7.Client.FormComponents.Internals.FormFactoryHelpers;

//added

//todo at some point this whole FormFieldFactory, GenericField thing needs refactoring
public class FormFieldFactory<TModel>
{
    FormFactory<TModel> form { get; }

    public FormFieldFactory(FormFactory<TModel> form)
    {
        this.form = form;
    }

    public List<GenericField<TModel>> Create()
    {
        PropertyInfo[] propertyInfos = typeof(TModel).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

        List<GenericField<TModel>> result = new(propertyInfos.Length);

        foreach (PropertyInfo propInfo in propertyInfos)
        {
            //skip props with no get;
            if (propInfo.SetMethod == null) continue;

            // [Editable(false)]
            if (propInfo.GetCustomAttribute<EditableAttribute>() is { } editor
                && !editor.AllowEdit) continue;

            //the dev doesn't want this property to be used in the form
            FormFactoryIgnore ignoreAttribute = propInfo.GetCustomAttribute<FormFactoryIgnore>();
            if (ignoreAttribute != null) continue;

            //this will (should) be a List<T> or Array[T] that will be the source for HTML option elements in an HTML select element, so we don't want to get a generic field for this one.
            FormFactoryKeyValueAttribute source = propInfo.GetCustomAttribute<FormFactoryKeyValueAttribute>();
            if (source != null) continue;

            result.Add(new GenericField<TModel>(form, propInfo));
        }

        return result;
    }

}
