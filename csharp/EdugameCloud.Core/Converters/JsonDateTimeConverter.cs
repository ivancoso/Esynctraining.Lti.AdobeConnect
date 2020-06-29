namespace EdugameCloud.Core.Converters
{
        using System;
        using System.Globalization;
        using System.Linq;
        using System.Text.RegularExpressions;

        using Esynctraining.Core.Extensions;

        using Newtonsoft.Json;
        using Newtonsoft.Json.Converters;

        /// <summary>
        ///     The JSON date time converter.
        /// </summary>
        public sealed class JsonDateTimeConverter : DateTimeConverterBase
        {
            #region Public Methods and Operators

            /// <summary>
            /// The convert from custom JSON.
            /// </summary>
            /// <param name="json">
            /// The JSON.
            /// </param>
            /// <param name="timeZoneOffset">
            /// The time zone offset.
            /// </param>
            /// <returns>
            /// The <see cref="DateTime"/>.
            /// </returns>
            public static DateTime ConvertFromCustomJson(string json, float? timeZoneOffset)
            {
                Match r =
                    new Regex(
                        @"(?<year>(\d){4})-(?<month>(\d){2})-(?<day>(\d){2})T(?<hour>(\d){2}):(?<min>(\d){2}):(?<sec>(\d){2})")
                        .Match(json);
                TimeZoneInfo targetTimeZone =
                    TimeZoneInfo.GetSystemTimeZones()
                                .FirstOrDefault(
                                    x => Math.Abs(x.BaseUtcOffset.ToFloat() - (timeZoneOffset ?? -8f)) < float.Epsilon);
                var dt = new DateTimeOffset(
                    r.GetInt("year"),
                    r.GetInt("month"),
                    r.GetInt("day"),
                    r.GetInt("hour"),
                    r.GetInt("min"),
                    r.GetInt("sec"),
                    targetTimeZone.Return(x => x.BaseUtcOffset, new TimeSpan(-8, 0, 0)));
                return dt.DateTime;
            }

            /// <summary>
            /// The read JSON.
            /// </summary>
            /// <param name="reader">
            /// The reader.
            /// </param>
            /// <param name="objectType">
            /// The object type.
            /// </param>
            /// <param name="existingValue">
            /// The existing value.
            /// </param>
            /// <param name="serializer">
            /// The serializer.
            /// </param>
            /// <returns>
            /// The <see cref="object"/>.
            /// </returns>
            /// <exception cref="NotImplementedException">
            ///  Not implemented
            /// </exception>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// The write JSON.
            /// </summary>
            /// <param name="writer">
            /// The writer.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <param name="serializer">
            /// The serializer.
            /// </param>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var dt = (DateTime)value;
                writer.WriteValue(dt.ToString("s", CultureInfo.InvariantCulture) + dt.ToString("zzz"));
            }

            #endregion
        }

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
