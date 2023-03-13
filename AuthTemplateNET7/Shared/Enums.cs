using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared;

//added

//Need this here because some of these are used in the database

public enum BatchStatus : byte
{
    InProgress,
    Paused,
    Complete
}

/// <summary>
/// Used for Bootstrap buttons, alerts, text (text-danger, etc), tables (table-info etc) etc
/// </summary>
public enum BootstrapColor : byte
{
    None,
    Danger,
    Info,
    Primary,
    Secondary,
    Success,
    Warning,
}

public enum EmailSendResult : byte //because it will be in the db
{
    Pending,
    Success,
    Error
}

public enum ValidationStatus : byte
{
    NotRun,
    Valid,
    Invalid
}

public enum Priority : byte
{
    Low,
    Medium,
    High
}

public enum RegistrationResult
{
    Succeeded,
    EmailAddressExists,
    DisplayNameExists,
    EmailAndDisplayExist,
    ServerError
}
