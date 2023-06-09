﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;

//added

//todo EmailSettings should be run through unit tests
public class EmailSettings
{
    bool timesAreUpdated = false;

    #region editable props

    public bool EmailingOn { get; set; }

    public int? MaxPerMinute { get; set; } = 9;

    public int? MaxPerHour { get; set; } = 45;

    public int? MaxPerDay { get; set; }

    public int? MaxPerMonth { get; set; }

    #endregion //editable props

    #region accumulators

    [FormFactoryIgnore, Obsolete("Public setter for ef, json only")]
    public int Month { get; set; }
    [FormFactoryIgnore, Obsolete("Public setter for ef, json only")]
    public int SentThisMonthCount { get; set; }

    [FormFactoryIgnore, Obsolete("Public setter for ef, json only")]
    public int DayOfMonth { get; set; }
    [FormFactoryIgnore, Obsolete("Public setter for ef, json only")]
    public int SentThisDayOfMonthCount { get; set; }

    [FormFactoryIgnore, Obsolete("Public setter for ef, json only")]
    public int Hour { get; set; }
    [FormFactoryIgnore, Obsolete("Public setter for ef, json only")]
    public int SentInLastHourCount { get; set; }

    [FormFactoryIgnore, Obsolete("Public setter for ef, json only")]
    public int Minute { get; set; }
    [FormFactoryIgnore, Obsolete("Public setter for ef, json only")]
    public int SentInLastMinuteCount { get; set; }

    #endregion //accumulators


#pragma warning disable CS0618

    #region UI

    public string AvailableMonth => MaxPerMonth.HasValue ? (MaxPerMonth.Value - SentThisMonthCount).ToString("N0") : "N/A";

    public string AvailableDay => MaxPerDay.HasValue ? (MaxPerDay.Value - SentThisDayOfMonthCount).ToString("N0") : "N/A";

    public string AvailableHour => MaxPerHour.HasValue ? (MaxPerHour.Value - SentInLastHourCount).ToString("N0") : "N/A";

    public string AvailableMinute => MaxPerMinute.HasValue ? (MaxPerMinute.Value - SentInLastMinuteCount).ToString("N0") : "N/A";

    #endregion //UI
    public int AvailableToSend()
    {
        if (!timesAreUpdated) throw new InvalidOperationException("EmailSettings.UpdateTimes() MUST BE CALLED BEFORE CALLING AvailableToSend()");

        int result = 0;
        int temp = 0;

        if (MaxPerMonth.HasValue)
        {
            result = MaxPerMonth.Value - SentThisMonthCount;
        }
        else
        {
            result = int.MaxValue;
        }

        if (MaxPerDay.HasValue)
        {
            temp = MaxPerDay.Value - SentThisDayOfMonthCount;
            if(temp < result) result = temp;
        }

        if (MaxPerHour.HasValue)
        {
            temp = MaxPerHour.Value - SentInLastHourCount;
            if(temp < result) result = temp;
        }

        if(MaxPerMinute.HasValue)
        {
            temp = MaxPerMinute.Value - SentInLastMinuteCount;
            if(temp < result) result = temp;
        }

        return result;
    }

    public void UpdateAccumulators(DateTime endedSendingTime, int emailsSentCount)
    {
        int month = endedSendingTime.Month;
        if(month == Month)
        {
            SentThisMonthCount += emailsSentCount;
        }
        else
        {
            Month = month;
            SentThisMonthCount = emailsSentCount;
        }

        int dayOfMonth = endedSendingTime.Day;
        if(dayOfMonth == DayOfMonth)
        {
            SentThisDayOfMonthCount += emailsSentCount;
        }
        else
        {
            DayOfMonth = dayOfMonth;
            SentThisDayOfMonthCount = emailsSentCount;
        }

        int hour = endedSendingTime.Hour;
        if(hour == Hour)
        {
            SentInLastHourCount += emailsSentCount;

            int minute = endedSendingTime.Minute;
            if(minute == Minute)
            {
                SentInLastMinuteCount += emailsSentCount;
            }
            else
            {
                Minute = minute;
                SentInLastMinuteCount = emailsSentCount;
            }
        }
        else
        {
            Hour = hour;
            SentInLastHourCount = emailsSentCount;

            Minute = endedSendingTime.Minute;
            SentInLastMinuteCount = emailsSentCount;
        }
    }

    public void UpdateTimes()
    {
        DateTime utcNow = DateTime.UtcNow;

        if (Month != utcNow.Month)
        {
            Month = utcNow.Month;
            SentThisMonthCount = 0;
        }

        if (DayOfMonth != utcNow.Day)
        {
            DayOfMonth = utcNow.Day;
            SentThisDayOfMonthCount = 0;
        }

        if (Hour != utcNow.Hour)
        {
            Hour = utcNow.Hour;
            SentInLastHourCount = 0;

            Minute = utcNow.Minute;
            SentInLastMinuteCount = 0;
        }
        else
        {
            if (Minute != utcNow.Minute)
            {
                Minute = utcNow.Minute;
                SentInLastMinuteCount = 0;
            }
        }

        timesAreUpdated = true;
    }

    public EmailSettings()
    {
        UpdateTimes();
    }
#pragma warning restore CS0618

    public static string Key = "EmailSettings";
}
