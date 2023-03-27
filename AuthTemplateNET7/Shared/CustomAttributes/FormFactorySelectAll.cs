using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.CustomAttributes;

//added

/// <summary>
/// All text will be selected when user clicks on input or textarea
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class FormFactorySelectAll : Attribute { }
