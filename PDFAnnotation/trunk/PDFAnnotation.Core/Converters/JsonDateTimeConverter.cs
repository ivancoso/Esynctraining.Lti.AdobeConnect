﻿namespace PDFAnnotation.Core.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Esynctraining.Core.Domain.Entities;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    //public class JsonDateTimeConverter : DateTimeConverterBase
    //{
    //    public static DateTimeWithZone ConvertFromCustomJson(string json, string dotNetId, float? timeZoneOffset)
    //    {
    //        Match r = new Regex(@"(?<year>(\d){4})-(?<month>(\d){2})-(?<day>(\d){2})T(?<hour>(\d){2}):(?<min>(\d){2}):(?<sec>(\d){2})").Match(json);
    //        TimeZoneInfo targetTimeZone = GetTimezone(dotNetId, timeZoneOffset);
    //        return new DateTimeWithZone(new DateTime(r.GetInt("year"), r.GetInt("month"), r.GetInt("day"), r.GetInt("hour"), r.GetInt("min"), r.GetInt("sec"), DateTimeKind.Unspecified), targetTimeZone);
    //    }
        
    //    public static TimeZoneInfo GetTimezone(string dotNetId, float? timeZoneOffset)
    //    {
    //        TimeZoneInfo res = null;
    //        if (!string.IsNullOrWhiteSpace(dotNetId))
    //        {
    //            try
    //            {
    //                res = TimeZoneInfo.FindSystemTimeZoneById(dotNetId);
    //            }
    //            catch 
    //            {

    //            }
    //        }

    //        if (res == null)
    //        {
    //            res = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => Math.Abs(x.BaseUtcOffset.ToFloat() - (timeZoneOffset ?? -5f)) < float.Epsilon);
    //        }
    //        return res;
    //    }

        
    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var dt = (DateTime)value;
    //        writer.WriteValue(dt.ToString("s", CultureInfo.InvariantCulture) + dt.ToString("zzz"));
    //    }

    //}

    /// <summary>
    /// The JSON extensions.
    /// </summary>
    public static class JsonExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets integer from group.
        /// </summary>
        /// <param name="match">
        /// The match.
        /// </param>
        /// <param name="groupName">
        /// The group name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetInt(this Match match, string groupName)
        {
            try
            {
                if (match.Groups[groupName] != null)
                {
                    return ToInt(match.Groups[groupName].Value);
                }
            }
            catch
            {
            }

            return default(int);
        }

        /// <summary>
        /// Convert time span to float.
        /// </summary>
        /// <param name="ts">
        /// The time span.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float ToFloat(this TimeSpan ts)
        {
            return ts.Hours + (ts.Minutes / 60f);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts string to integer.
        /// </summary>
        /// <param name="val">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int ToInt(string val)
        {
            int res;
            if (int.TryParse(val, out res))
            {
                return res;
            }

            return default(int);
        }

        #endregion
    }
}