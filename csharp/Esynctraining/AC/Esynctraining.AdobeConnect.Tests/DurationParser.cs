using System;
using System.Text.RegularExpressions;

namespace Esynctraining.AdobeConnect.Tests
{
    public class DurationParser
    {
        public static TimeSpan Parse(string value)
        {
            TimeSpan ts;
            if (TimeSpan.TryParse(value, out ts))
            {
                return ts;
            }
            var pattern = @"(\d+)d\s+(\d+)\:(\d+)\:(\d+)\.(\d+)";
            var regex = new Regex(pattern);
            if (regex.Match(value).Success)
            {
                if (regex.Matches(value).Count > 1)
                    throw new InvalidOperationException(" too many matches for a string");
                var match = regex.Matches(value)[0];
                var days = int.Parse(match.Groups[1].Value);
                var hours = int.Parse(match.Groups[2].Value);
                var minutes = int.Parse(match.Groups[3].Value);
                var secs = int.Parse(match.Groups[4].Value);
                var millisecs = int.Parse(match.Groups[5].Value);
                return new TimeSpan(days, hours, minutes, secs, millisecs);
            }
            throw new InvalidOperationException("");
        }
    }
}