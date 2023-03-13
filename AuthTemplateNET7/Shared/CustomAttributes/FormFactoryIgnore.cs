using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.CustomAttributes;

//added

/// <summary>
/// Property will be skipped by the Client.FormComponents.FormFactory component
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FormFactoryIgnore : Attribute { }
