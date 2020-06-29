using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.Core.Business.MeetingNameFormatting.Formatters;
using Esynctraining.Core.Json;

namespace EdugameCloud.Lti.Core.Business.MeetingNameFormatting
{
    public interface IMeetingNameFormatterFactory
    {
        IEnumerable<KeyValuePair<int, string>> GetFormatters();

        IMeetingNameFormatter GetFormatter(int formatterId);
    }
    
    public class MeetingNameFormatterFactory : IMeetingNameFormatterFactory
    {
        private readonly Dictionary<int, IMeetingNameFormatter> _formatters;


        public static readonly int DefaultFormatterId = 2;


        public MeetingNameFormatterFactory(IJsonSerializer nameInfoSerializer, IJsonDeserializer nameInfoDeserializer)
        {
            if (nameInfoSerializer == null)
                throw new ArgumentNullException(nameof(nameInfoSerializer));
            if (nameInfoDeserializer == null)
                throw new ArgumentNullException(nameof(nameInfoDeserializer));


            _formatters = new Dictionary<int, IMeetingNameFormatter> 
            {
                { 1, new StraightMeetingNameFormatter(nameInfoSerializer, nameInfoDeserializer) },
                { 2, new CourseIdPrefixMeetingNameFormatter(nameInfoSerializer, nameInfoDeserializer) },
                { 3, new CourseNumPrefixMeetingNameFormatter(nameInfoSerializer, nameInfoDeserializer) },
                { 4, new CourseNumPrefixIdSuffixMeetingNameFormatter(nameInfoSerializer, nameInfoDeserializer) },
                { 5, new CourseNumPrefixDateTimeSuffixMeetingNameFormatter(nameInfoSerializer, nameInfoDeserializer) },
                { 6, new CourseNumPrefixDateTimeSuffixMeetingNameFormatter(nameInfoSerializer, nameInfoDeserializer, "dd/MM/yy") },
            };
        }


        public IEnumerable<KeyValuePair<int, string>> GetFormatters()
        {
            return _formatters.Select(x => new KeyValuePair<int, string>(x.Key, x.Value.FormatName));
        }

        public IMeetingNameFormatter GetFormatter(int formatterId)
        {
            if (formatterId <= 0)
                throw new ArgumentOutOfRangeException(nameof(formatterId));

            IMeetingNameFormatter result;
            if (_formatters.TryGetValue(formatterId, out result))
                return result;

            throw new InvalidOperationException(string.Format("Not supported formatter id: {0}", formatterId.ToString()));
        }

    }

}
