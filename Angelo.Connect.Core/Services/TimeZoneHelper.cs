using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public static class TimeZoneHelper
    {
        public static List<SelectListItem> GetTimeZoneSelectList()
        {
            return GetTimeZoneSelectList(new string[] { });
        }

        public static List<SelectListItem> GetTimeZoneSelectList(string selectedTimeZoneId)
        {
            return GetTimeZoneSelectList(new string[] { selectedTimeZoneId });
        }

        /// <summary>
        /// Returns a SelectList containing the names and ids of the system time zones.
        /// </summary>
        public static List<SelectListItem> GetTimeZoneSelectList(string[] selectedTimeZoneIds)
        {
            ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
            var list = new List<SelectListItem>();
            foreach (TimeZoneInfo timeZone in timeZones)
            {
                list.Add(new SelectListItem()
                {
                    Value = timeZone.Id,
                    Text = timeZone.DisplayName,
                    Selected = selectedTimeZoneIds.Contains(timeZone.Id)
                });
            }
            return list;
        }

        public static string DefaultTimeZoneId
        {
            get { return TimeZoneInfo.Local.Id; }
        }

        public static DateTime Now(string timeZoneId)
        {
            var timeZoneInfo = GetTimeZoneInfo(timeZoneId);
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, timeZoneInfo);
        }

        public static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        public static TimeZoneInfo GetTimeZoneInfo(string timeZoneId, TimeZoneInfo defaultTimeZoneInfo)
        {
            if (string.IsNullOrEmpty(timeZoneId))
                return defaultTimeZoneInfo;

            TimeZoneInfo timeZoneInfo = GetTimeZoneInfo(timeZoneId);
            if (timeZoneInfo == null)
                return defaultTimeZoneInfo;
            else
                return timeZoneInfo;
        }

        /// <summary>
        /// Returns the Display Name of the Time Zone having the specified Id.
        /// </summary>
        public static string NameOfTimeZoneId(string timeZoneId, string defaultName = "")
        {
            if (string.IsNullOrEmpty(timeZoneId))
                return defaultName;

            var timeZoneInfo = GetTimeZoneInfo(timeZoneId);
            if (timeZoneInfo == null)
                return defaultName;
            else
                return timeZoneInfo.DisplayName;
        }

        /// <summary>
        /// Converts the source DateTime to the UTC DateTime.
        /// </summary>
        public static DateTime ConvertToUTC(DateTime sourceDateTime, string sourceTimeZoneId)
        {
            var sourceTimeZone = GetTimeZoneInfo(sourceTimeZoneId);
            if (sourceTimeZone == null)
                throw new Exception("Unable to convert to UTC.  TimeZoneInfo not found (TimeZoneId: " + sourceTimeZoneId + ").");
            return TimeZoneInfo.ConvertTime(sourceDateTime, sourceTimeZone, TimeZoneInfo.Utc);
        }

        /// <summary>
        /// Converts the source DateTime to the UTC DateTime.
        /// </summary>
        public static DateTime ConvertToUTC(DateTime sourceDateTime, string sourceTimeZoneId, DateTime defaultDateTime)
        {
            try
            {
                var sourceTimeZone = GetTimeZoneInfo(sourceTimeZoneId);
                if (sourceTimeZone == null)
                    return defaultDateTime;

                return TimeZoneInfo.ConvertTime(sourceDateTime, sourceTimeZone, TimeZoneInfo.Utc);
            } catch
            {
                return defaultDateTime;
            }
        }

        /// <summary>
        /// Converts the UTC DateTime to the DateTime of the specified TimeZone.
        /// </summary>
        public static DateTime ConvertFromUTC(DateTime utc, string destTimeZoneId)
        {
            var destTimeZone = GetTimeZoneInfo(destTimeZoneId);
            if (destTimeZone == null)
                throw new Exception("Unable to convert from UTC.  TimeZoneInfo not found (TimeZoneId: " + destTimeZoneId + ").");
            return TimeZoneInfo.ConvertTime(utc, TimeZoneInfo.Utc, destTimeZone);
        }

        /// <summary>
        /// Converts the UTC DateTime to the DateTime of the specified TimeZone.
        /// </summary>
        public static DateTime ConvertFromUTC(DateTime utc, string destTimeZoneId, DateTime defaultDateTime)
        {
            try
            {
                var destTimeZone = GetTimeZoneInfo(destTimeZoneId);
                if (destTimeZone == null)
                    return defaultDateTime;
                return TimeZoneInfo.ConvertTime(utc, TimeZoneInfo.Utc, destTimeZone);
            } catch
            {
                return defaultDateTime;
            }
        }




    }
}
