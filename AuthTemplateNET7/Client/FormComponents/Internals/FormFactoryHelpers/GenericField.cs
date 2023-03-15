using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Forms;
using AuthTemplateNET7.Client.FormComponents.Internals.FormFactoryHelpers.CustomInputs;
using System.Collections.Generic;
using System.Collections;
using AuthTemplateNET7.Shared.CustomAttributes;
using System.Data;
using AuthTemplateNET7.Shared.SharedServices;

namespace AuthTemplateNET7.Client.FormComponents.Internals.FormFactoryHelpers;

//added
public class GenericField<TModel>
{
    //todo ASAP remove all sequence++ and hardcode (these are in your custom components too) https://chrissainty.com/building-components-via-rendertreebuilder/
    public GenericField(FormFactory<TModel> form, PropertyInfo propertyInfo)
    {
        this.form_ = form;
        model_ = form.Model;
        this.propertyInfo_ = propertyInfo;
        this.dataTypeAttribute_ = propertyInfo.GetCustomAttribute<DataTypeAttribute>();

        displayAttribute_ = propertyInfo.GetCustomAttribute<DisplayAttribute>();
        setDisplayName();

        RequiredAttribute requiredAttribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
        if (requiredAttribute != null) Required = true;

        setPrompt();

        propertyType_ = propertyInfo.PropertyType;

        if (propertyType_ == typeof(bool) || propertyType_ == typeof(bool?))
        {
            if (dataTypeAttribute_ != null
                && string.Equals(
                    dataTypeAttribute_.CustomDataType,
                    "Switch",
                    StringComparison.OrdinalIgnoreCase))
            {
                FormSwitch = "form-switch";
            }

            FieldSelector = FieldSelector.Check;
        }

        FormFactorySelectSourceAttribute selectSourceAttribute = propertyInfo.GetCustomAttribute<FormFactorySelectSourceAttribute>();

        if(selectSourceAttribute != null)
        {
            FieldSelector = FieldSelector.Select;
            selectSourceAttributeName_ = selectSourceAttribute.GetSourcePropertyName();
        }

        s_eventCallbackFactoryCreate = getEventCallbackFactoryCreate();
        this.dataTypeAttribute_ = dataTypeAttribute_;
    }

    #region fields

    DisplayAttribute displayAttribute_ { get; }

    DataTypeAttribute dataTypeAttribute_ { get; }

    FormFactory<TModel> form_ { get; }

    TModel model_ { get; }

    /// <summary>
    /// For the "placeholder" attribute on the input
    /// </summary>
    string prompt_ { get; set; }

    PropertyInfo propertyInfo_ { get; }

    Type propertyType_ { get; }

    //for generating select elements in prop public RenderFragment SelectEditorTemplate
    string selectSourceAttributeName_ { get; }

    static MethodInfo s_eventCallbackFactoryCreate { get; set; }

    #endregion //fields

    #region props

    public string DisplayName { get; private set; }

    public string Description
    {
        get
        {
            DisplayAttribute displayAttribute = propertyInfo_.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                var description = displayAttribute.GetDescription();
                if (!string.IsNullOrEmpty(description)) return description;
            }

            var descriptionAttribute = propertyInfo_.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
            {
                var description = descriptionAttribute.Description;
                if (!string.IsNullOrEmpty(description)) return description;
            }

            return null;
        }
    }

    public RenderFragment EditorTemplate
    {
        get
        {
            // () => Owner.Property
            MemberExpression expression = Expression.Property(
                Expression.Constant(model_, typeof(TModel)),
                propertyInfo_);

            LambdaExpression lambda = Expression.Lambda(
                typeof(Func<>).MakeGenericType(propertyType_),
                expression);

            // Create(object receiver, Action<object> callback
            MethodInfo methodInfo = s_eventCallbackFactoryCreate.MakeGenericMethod(propertyType_);

            // value => Field.Value = value;
            ParameterExpression changeHandlerParameter = Expression.Parameter(propertyType_);

            BinaryExpression body = Expression.Assign(
                Expression.Property(
                    Expression.Constant(this),
                    nameof(Value)),
                Expression.Convert(changeHandlerParameter, typeof(object)));

            LambdaExpression changeHandlerLambda = Expression.Lambda(
                typeof(Action<>)
                .MakeGenericType(propertyType_),
                body,
                changeHandlerParameter);

            object changeHandler = methodInfo.Invoke(
                EventCallback.Factory,
                new object[] { this, changeHandlerLambda.Compile() });

            return builder =>
            {
                var (componentType, classAttributeValue, htmlInputType, additionalAttributes) = getEditorType();
                int sequence = 0;

                builder.OpenComponent(sequence++, componentType);

                if(additionalAttributes != null)
                {
                    builder.AddMultipleAttributes(sequence++, additionalAttributes);
                }

                if(propertyType_.IsEnum)
                {
                    builder.AddAttribute(sequence++, "Required", Required);
                }

                if (htmlInputType != null) builder.AddAttribute(sequence++, "type", htmlInputType);

                builder.AddAttribute(sequence++, "Value", Value);

                builder.AddAttribute(sequence++, "ValueChanged", changeHandler);

                builder.AddAttribute(sequence++, "ValueExpression", lambda);

                builder.AddAttribute(sequence++, "id", InputId);

                if (prompt_ != null) builder.AddAttribute(sequence++, "placeholder", prompt_);

                FormFactorySelectAll formFactorySelectAll = propertyInfo_.GetCustomAttribute<FormFactorySelectAll>();
                if (formFactorySelectAll != null)
                {
                    builder.AddAttribute(sequence++, "select-all", "");
                }

                if(classAttributeValue != null) builder.AddAttribute(sequence++, "class", classAttributeValue);

                //builder.AddMultipleAttributes(6, additonalAttributes);

                builder.CloseComponent();
            };
        }
    }

    /// <summary>
    /// Determines Bootstrap classes, e.g. "form-control", "form-check", "form-select"
    /// </summary>
    public FieldSelector FieldSelector { get; }

    /// <summary>
    /// For checkbox inputs to be displayed as switches
    /// </summary>
    public string FormSwitch { get; }

    public string InputId { get; } = StringHelpers.GenerateRandomString();

    public bool Required { get; }

    public RenderFragment SelectEditorTemplate
    {
        get
        {
            #region get IList<T> values

            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/5cbced13-14ef-4afd-9dad-fd330e4b9b8a/how-to-access-the-propertyinfo-of-generic-list-collection-using-reflection-?forum=csharpgeneral

            var sourcePropInfo = typeof(TModel).GetProperty(selectSourceAttributeName_);

            if(sourcePropInfo == null)
            {
                throw new ArgumentNullException($"FORMFACTORY: PROPERTY {selectSourceAttributeName_} WAS NOT FOUND ON {model_.GetType().FullName}. Consider using 'nameof(PropertyName)'.");
            }

            IList sourceList = (IList)sourcePropInfo.GetValue(form_.Model, null);

            if(sourceList == null)
            {
                throw new ArgumentNullException($"FORMFACTORY: {selectSourceAttributeName_} PROPERTY ON {model_.GetType().FullName} CAN NOT BE NULL.");
            }

            FormFactoryKeyValueAttribute keyValueAttribute = sourcePropInfo
                .GetCustomAttribute<FormFactoryKeyValueAttribute>();

            if(keyValueAttribute == null)
            {
                throw new ArgumentNullException($"FORMFACTORY: PROPERTY {selectSourceAttributeName_} ON {model_.GetType().FullName} MUST HAVE A 'FormFactoryKeyValue' ATTRIBUTE");
            }

            string keyPropertyName = keyValueAttribute.GetPropertyKeyName();
            string valuePropertyName = keyValueAttribute.GetPropertyValueName();

            Dictionary<string, string> optionsDict = new Dictionary<string, string>(sourceList.Count);

            foreach(object obj in sourceList)
            {
                var keyProp = obj.GetType().GetProperty(keyPropertyName);
                var keyValue = keyProp.GetValue(obj).ToString();

                var valueProp = obj.GetType().GetProperty(valuePropertyName);
                var valueValue = valueProp.GetValue(obj, null).ToString();

                optionsDict.Add(keyValue, valueValue);
            }

            #endregion //get IList<T> values

            #region changeHandler

            // () => Owner.Property
            MemberExpression expression = Expression.Property(
                Expression.Constant(model_, typeof(TModel)),
                propertyInfo_);

            LambdaExpression lambda = Expression.Lambda(
                typeof(Func<>).MakeGenericType(propertyType_),
                expression);

            // Create(object receiver, Action<object> callback
            MethodInfo methodInfo = s_eventCallbackFactoryCreate.MakeGenericMethod(propertyType_);

            // value => Field.Value = value;
            ParameterExpression changeHandlerParameter = Expression.Parameter(propertyType_);

            BinaryExpression body = Expression.Assign(
                Expression.Property(
                    Expression.Constant(this),
                    nameof(Value)),
                Expression.Convert(changeHandlerParameter, typeof(object)));

            LambdaExpression changeHandlerLambda = Expression.Lambda(
                typeof(Action<>)
                .MakeGenericType(propertyType_),
                body,
                changeHandlerParameter);

            object changeHandler = methodInfo.Invoke(
                EventCallback.Factory,
                new object[] { this, changeHandlerLambda.Compile() });

            #endregion //changeHandler

            return builder =>
            {
                int sequence = 0;

                Type type = typeof(SelectInputCustom<>).MakeGenericType(propertyType_);

                builder.OpenComponent(sequence++, type);

                //hack not sure why it doesn't pick up the CurrentValue...
                builder.AddAttribute(sequence++, "PassedInValue", Value?.ToString());
                builder.AddAttribute(sequence++, "CurrentValue", Value);
                builder.AddAttribute(sequence++, "ValueChanged", changeHandler);
                builder.AddAttribute(sequence++, "ValueExpression", lambda);

                builder.AddAttribute(sequence++, "OptionElements", optionsDict);
                builder.AddAttribute(sequence++, "Required", Required);

                string placeholder = "Please select...";
                if (displayAttribute_ != null)
                {
                    var prompt = displayAttribute_.GetPrompt();
                    if (prompt != null)
                    {
                        placeholder = prompt;
                    }
                }

                builder.AddAttribute(sequence++, "Placeholder", placeholder);

                builder.CloseComponent();
            };
        }
    }

    public RenderFragment ValidationMessageTemplate
    {
        get
        {
            if(!form_.DisplayValidationMessages) return null;

            return builder =>
            {
                MemberExpression access = Expression.Property(
                    Expression.Constant(model_, typeof(TModel)),
                    propertyInfo_);

                LambdaExpression lambda = Expression.Lambda(
                    typeof(Func<>).MakeGenericType(propertyType_),
                    access);

                builder.OpenComponent(0,
                    typeof(ValidationMessage<>).MakeGenericType(propertyType_));

                builder.AddAttribute(1, "For", lambda);
                builder.CloseComponent();
            };
        }
    }

    public object Value
    {
        get => propertyInfo_.GetValue(model_);

        set
        {
            if (propertyInfo_.SetMethod != null)
            {
                propertyInfo_.SetValue(model_, value);
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler ValueChanged;

    #endregion //props

    #region helper methods

    (Type ComponentType, string classAttributeValue, string inputType, IEnumerable<KeyValuePair<string, object>> additionalAttributes) getEditorType()
    {
        //inputmode="numeric"
        string formControl = "form-control";

        IEnumerable<EditorAttribute> editorAttributes = propertyInfo_
            .GetCustomAttributes<EditorAttribute>();

        foreach (var editorAttribute in editorAttributes)
        {
            if (editorAttribute.EditorBaseTypeName == typeof(InputBase<>).AssemblyQualifiedName)
                return (Type.GetType(editorAttribute.EditorTypeName, throwOnError: true)!, null, null, null);
        }

        if (propertyType_ == typeof(string))
        {
            if (dataTypeAttribute_ != null)
            {
                if (dataTypeAttribute_.DataType == DataType.EmailAddress)
                {
                    return (typeof(InputText), formControl, "email", new[] {KeyValuePair.Create<string, object>("inputmode", "email")});
                }

                if (dataTypeAttribute_.DataType == DataType.PhoneNumber)
                {
                    return (typeof(InputText), formControl, "tel", new[] { KeyValuePair.Create<string, object>("inputmode", "tel") });
                }

                if (dataTypeAttribute_.DataType == DataType.Password)
                {
                    return (typeof(InputText), formControl, "password", null);
                }

                if (dataTypeAttribute_.DataType == DataType.MultilineText)
                {
                    return (typeof(InputTextArea), formControl, null, null);
                }

                if (dataTypeAttribute_.DataType == DataType.Url)
                {
                    return (typeof(InputText), formControl, "url", new[] { KeyValuePair.Create<string, object>("inputmode", "url") });
                }

                if (dataTypeAttribute_.DataType == DataType.Time)
                {
                    return (typeof(InputText), formControl, "time", null);
                }

                if (dataTypeAttribute_.DataType == DataType.Date)
                {
                    return (typeof(InputText), formControl, "date", null);
                }

                if (dataTypeAttribute_.DataType == DataType.DateTime)
                {
                    return (typeof(InputText), formControl, "datetime-local", null);
                }

                //todo use at some point dataType.CustomDataType for inline checkboxes, inline radios
                if (string.Equals(
                    dataTypeAttribute_.CustomDataType,
                    "Color",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return (typeof(InputText), "form-control form-control-color", "color", null);
                }
            }

            return (typeof(InputText), formControl, null, null);
        }

        #region numbers

        //Microsoft's InputNumber doesn't support byte and byte?
        if (propertyType_ == typeof(int)
            || propertyType_ == typeof(int?)
            || propertyType_ == typeof(decimal)
            || propertyType_ == typeof(decimal?)
            || propertyType_ == typeof(long)
            || propertyType_ == typeof(long?)
            || propertyType_ == typeof(double)
            || propertyType_ == typeof(double?)
            || propertyType_ == typeof(float)
            || propertyType_ == typeof(float?)
            || propertyType_ == typeof(byte)
            || propertyType_ == typeof(byte?)
            || propertyType_ == typeof(short)
            || propertyType_ == typeof(short?)
            )
        {
            return (typeof(NumberInputCustom<>).MakeGenericType(propertyType_), null, null, new[] { KeyValuePair.Create<string, object>("inputmode", "decimal") });
        }

        #endregion //numbers

        #region bools

        if (propertyType_ == typeof(bool))
        {
            return (typeof(InputCheckbox), "form-check-input cursor-pointer", "checkbox", null);
        }
        else if (propertyType_ == typeof(bool?))
        {
            return (typeof(NullableBoolCustom), "form-check-input cursor-pointer", "checkbox", null);
        }

        #endregion //bools

        #region date types

        bool isDate = false;
        Type returnType = null;
        string css = null;

        if (propertyType_ == typeof(DateTime))
        {
            isDate = true;

            if (dataTypeAttribute_ != null && dataTypeAttribute_.DataType == DataType.Date)
            {
                returnType = typeof(InputDate<DateTime>);
                css = "form-control";
            }
            else returnType = typeof(DateTimeCustom<DateTime>);
        }
        else if (propertyType_ == typeof(DateTime?))
        {
            isDate = true;

            if (dataTypeAttribute_ != null && dataTypeAttribute_.DataType == DataType.Date)
            {
                returnType = typeof(InputDate<DateTime?>);
                css = "form-control";
            }
            else returnType = typeof(DateTimeCustom<DateTime?>);
        }

        else if (propertyType_ == typeof(DateTimeOffset))
        {
            isDate = true;

            if (dataTypeAttribute_ != null && dataTypeAttribute_.DataType == DataType.Date)
            {
                css = "form-control";
                returnType = typeof(InputDate<DateTimeOffset>);
            }
            else returnType = typeof(DateTimeCustom<DateTimeOffset>);
        }
        else if (propertyType_ == typeof(DateTimeOffset?))
        {
            isDate = true;

            if (dataTypeAttribute_ != null && dataTypeAttribute_.DataType == DataType.Date)
            {
                css = "form-control";
                returnType = typeof(InputDate<DateTimeOffset?>);
            }
            else returnType = typeof(DateTimeCustom<DateTimeOffset?>);
        }

        else if (propertyType_ == typeof(DateOnly))
        {
            isDate = true;
            returnType = typeof(DateOnlyCustom<DateOnly>);
        }
        else if (propertyType_ == typeof(DateOnly?))
        {
            isDate = true;
            returnType = typeof(DateOnlyCustom<DateOnly?>);
        }

        if (isDate)
        {
            return (returnType, css, null, null);
        }

        #endregion //date types

        #region enums

        if (propertyType_.IsEnum
            && !propertyType_.IsDefined(typeof(FlagsAttribute), inherit: true))
        {
            return (typeof(EnumSelectCustom<>).MakeGenericType(propertyType_), "form-select", null, null);
        }

        if (Nullable.GetUnderlyingType(propertyType_)?.IsEnum == true
            && !propertyType_.IsDefined(typeof(FlagsAttribute), inherit: true))
        {
            return (typeof(EnumSelectCustom<>).MakeGenericType(propertyType_), "form-select", null, null);
        }

        //if (propertyType_.IsEnum || Nullable.GetUnderlyingType(propertyType_)?.IsEnum == true)
        //{
        //    if (!propertyType_.IsDefined(typeof(FlagsAttribute), inherit: true))
        //        return (typeof(InputEnumSelect<>).MakeGenericType(propertyType_), "form-select", null);
        //}

        #endregion //enums

        #region timeonly

        if (propertyType_ == typeof(TimeOnly))
        {
            return (typeof(TimeOnlyCustom<TimeOnly>), formControl, null, null);
        }

        if (propertyType_ == typeof(TimeOnly?))
        {
            return (typeof(TimeOnlyCustom<TimeOnly?>), formControl, null, null);
        }

        #endregion //timeonly

        return (typeof(TextInput), formControl, null, null);
    }

    static MethodInfo getEventCallbackFactoryCreate()
    {
        return typeof(EventCallbackFactory).GetMethods()
            .Single(m =>
            {
                if (m.Name != "Create"
                    || !m.IsPublic
                    || m.IsStatic
                    || !m.IsGenericMethod) return false;

                Type[] genericArgs = m.GetGenericArguments();
                if (genericArgs.Length != 1) return false;

                ParameterInfo[] args = m.GetParameters();

                return args.Length == 2
                    && args[0].ParameterType == typeof(object)
                    && args[1].ParameterType.IsGenericType
                    && args[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>);
            });
    }

    void setDisplayName()
    {
        if (displayAttribute_ != null)
        {
            var displayName = displayAttribute_.GetName();
            if (!string.IsNullOrEmpty(displayName))
            {
                DisplayName = displayName;
                return;
            }
        }

        DisplayNameAttribute displayNameAttribute = propertyInfo_.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttribute != null)
        {
            var displayName = displayNameAttribute.DisplayName;
            Console.WriteLine("DNA != null:" + displayName.Length);
            if (!string.IsNullOrEmpty(displayName))
            {
                DisplayName = displayName;
                return;
            }
        }

        DisplayName = propertyInfo_.Name.TitleCaseToWords();
    }

    void setPrompt()
    {
        if(propertyType_ == typeof(bool)) return; //this is gonna jam on a bool that is displayed as two radio buttons

        if(displayAttribute_ != null)
        {
            var prompt = displayAttribute_.GetPrompt();
            if(prompt != null)
            {
                prompt_ = prompt;
                return;
            }
        }

        string requiredStr = Required ? "(Required) " : "";

        prompt_ = $"{requiredStr}Enter {DisplayName?.ToLower()} here...";
    }

    #endregion //helper methods
}

/// <summary>
/// Determine if FormFactory should use "form-control", "form-check" + "form-check-input", "form-select"
/// </summary>
public enum FieldSelector
{
    /// <summary>
    /// Standard Bootstrap input class "form-control"
    /// </summary>
    Control,

    /// <summary>
    /// Bootstrap "form-check", "form-check-label", "form-check-input", maybe "form-switch"
    /// </summary>
    Check,

    /// <summary>
    /// Bootstrap "form-select"
    /// </summary>
    Select,
}
