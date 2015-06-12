using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting
{
    // TODO: DI
    public static class MeetingNameFormatterFactory
    {
        private static readonly Dictionary<int, IMeetingNameFormatter> _formatters;


        public static readonly int DefaultFormatterId = 2;


        static MeetingNameFormatterFactory()
        {
            _formatters = new Dictionary<int, IMeetingNameFormatter> 
            {
                { 1, new StraightMeetingNameFormatter() },
                { 2, new CourseIdPrefixMeetingNameFormatter() },
                { 3, new CourseNumPrefixMeetingNameFormatter() },
                { 4, new CourseNumPrefixIdSuffixMeetingNameFormatter() },
                { 5, new CourseNumPrefixDateTimeSuffixMeetingNameFormatter() },
            };
        }


        public static IEnumerable<KeyValuePair<int, string>> GetFormatters()
        {
            return _formatters.Select(x => new KeyValuePair<int, string>(x.Key, x.Value.FormatName));
        }

        public static IMeetingNameFormatter GetFormatter(int formatterId)
        {
            if (formatterId <= 0)
                throw new ArgumentOutOfRangeException("formatterId");

            IMeetingNameFormatter result;
            if (_formatters.TryGetValue(formatterId, out result))
                return result;

            throw new InvalidOperationException(string.Format("Not supported formatter id: {0}", formatterId.ToString()));
        }

    }

}
