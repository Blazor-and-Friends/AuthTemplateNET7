using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.SharedServices;
public static class DateHelpers
{
    /// <summary>
    /// yyyy-MM-dd
    /// </summary>
    public static string To_yyyyMMdd(this DateOnly dateOnly)
    {
        return dateOnly.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// yyyy-MM-dd
    /// </summary>
    public static string To_yyyyMMdd(this DateOnly? dateOnly)
    {
        return dateOnly?.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// yyyy-MM-dd
    /// </summary>
    public static string To_yyyyMMdd(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// yyyy-MM-dd
    /// </summary>
    public static string To_yyyyMMdd(this DateTime? dateTime)
    {
        return dateTime?.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// yyyy-MM-ddTHH:mm
    /// </summary>
    public static string To_yyyyMMdd_T_HHmm(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm");
    }

    /// <summary>
    /// yyyy-MM-ddTHH:mm
    /// </summary>
    public static string To_yyyyMMdd_T_HHmm(this DateTime? dateTime)
    {
        return dateTime?.ToString("yyyy-MM-ddTHH:mm");
    }

    /// <summary>
    /// yyyy-MM-ddTHH:mm
    /// </summary>
    public static string To_yyyyMMdd_T_HHmm(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToString("yyyy-MM-ddTHH:mm");
    }

    /// <summary>
    /// yyyy-MM-ddTHH:mm
    /// </summary>
    public static string To_yyyyMMdd_T_HHmm(this DateTimeOffset? dateTimeOffset)
    {
        return dateTimeOffset?.ToString("yyyy-MM-ddTHH:mm");
    }

}
