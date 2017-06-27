using System;
using System.Collections.Generic;
using System.Text;

namespace Esynctraining.AC.Provider.Entities
{
    /// <summary>
    /// http://blogs.adobe.com/connectsupport/xml-api-tips-creating-telephony-profiles-via-the-xml-api/
    /// </summary>
    public interface ITelephonyProfileProviderFields
    {
        string ToQueryString();

    }

    public sealed class MeetingOneProviderFields : ITelephonyProfileProviderFields
    {
        private static readonly string FieldPrefix = "x-tel-meetingone-";


        /// <summary>
        /// MeetingOne Conference Room Number (conference-id)
        /// </summary>
        public string ConferenceId { get; set; }

        /// <summary>
        /// Host Access Code (host-pin)
        /// </summary>
        public string HostPin { get; set; }

        /// <summary>
        /// Participant Access Code (participant-code)
        /// </summary>
        public string ParticipantCode { get; set; }


        public string ToQueryString()
        {
            return new TelephonyProviderFieldRequestBuilder(FieldPrefix)
                .Add("conference-id", ConferenceId)
                .Add("host-pin", HostPin)
                .Add("participant-code", ParticipantCode)
                .BuildQueryParams();
        }

    }

    public sealed class ArkadinProviderFields : ITelephonyProfileProviderFields
    {
        private static readonly string FieldPrefix = "x-tel-arkadin-";


        /// <summary>
        /// Web login (conference-id)
        /// </summary>
        public string ConferenceId { get; set; }

        /// <summary>
        /// Moderator pin code (moderator-code)
        /// </summary>
        public string ModeratorCode { get; set; }

        ///// <summary>
        ///// Participant pin code (participant-code)
        ///// </summary>
        //public string ParticipantCode { get; set; }


        /// <summary>
        /// Toll access number (conference-number)
        /// </summary>
        public string ConferenceNumber { get; set; }

        /// <summary>
        /// Toll free access number (conference-number-free)
        /// </summary>
        public string ConferenceNumberFree { get; set; }

        /// <summary>
        /// SIP access number (conference-number-uvline)
        /// </summary>
        public string ConferenceNumberUvline { get; set; }


        public string ToQueryString()
        {
            return new TelephonyProviderFieldRequestBuilder(FieldPrefix)
                .Add("conference-id", ConferenceId)
                .Add("moderator-code", ModeratorCode)
                //.Add("participant-code", ParticipantCode)
                .Add("conference-number", ConferenceNumber)
                .Add("conference-number-free", ConferenceNumberFree)
                .Add("conference-number-uvline", ConferenceNumberUvline)
                .BuildQueryParams();
        }
    }


    public sealed class IntercallProviderFields : ITelephonyProfileProviderFields
    {
        private static readonly string FieldPrefix = "x-tel-intercall-";


        ///// <summary>
        ///// Conference Number (conference-number)
        ///// </summary>
        //public string ConferenceNumber { get; set; }

        /// <summary>
        /// Conference Code (participant-code)
        /// </summary>
        public string ConferenceCode { get; set; }

        /// <summary>
        /// Leader Pin (leader-pin)
        /// </summary>
        public string LeaderPin { get; set; }

        ///// <summary>
        ///// Further Dial In Numbers (company-url)
        ///// </summary>
        //public string FurtherDialInNumbers { get; set; }

        ///// <summary>
        ///// {x-tel-intercall-uv-conference-number} (uv-conference-number)
        ///// </summary>
        //public string UvConferenceNumber { get; set; }


        public string ToQueryString()
        {
            return new TelephonyProviderFieldRequestBuilder(FieldPrefix)
                //.Add("conference-number", ConferenceNumber)
                .Add("participant-code", ConferenceCode)
                .Add("leader-pin", LeaderPin)
                //.Add("company-url", FurtherDialInNumbers)
                //.Add("uv-conference-number", UvConferenceNumber)
                .BuildQueryParams();
        }
    }

    internal sealed class TelephonyProviderFieldRequestBuilder
    {
        private readonly string _fieldPrefix;
        private readonly IDictionary<string, string> _values;


        public TelephonyProviderFieldRequestBuilder(string fieldPrefix)
        {
            if (string.IsNullOrWhiteSpace(fieldPrefix))
                throw new ArgumentException("Non-empty value expected", nameof(fieldPrefix));
            _fieldPrefix = fieldPrefix;
            _values = new Dictionary<string, string>();
        }


        public TelephonyProviderFieldRequestBuilder Add(string field, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return this;

            _values.Add(field, value);
            return this;
        }

        public string BuildQueryParams()
        {
            var query = new StringBuilder(_values.Count * 25);
            foreach (KeyValuePair<string, string> value in _values)
            {
                //&field-id=x-tel-intercall-leader-pin&value=xxxxxx
                query.AppendFormat("&field-id={0}{1}&value={2}", _fieldPrefix, value.Key, HttpUtilsInternal.UrlEncode(value.Value));
            }
            return query.ToString();
        }

    }

}
