using System;
using System.Collections.Generic;
using System.Linq;

namespace EdugameCloud.Lti.Telephony
{
    public interface ITelephonyProfileHumanizer
    {
        IDictionary<string, string> Humanize(IDictionary<string, string> rawTelephonyProfileFields);

    }

    public class TelephonyProfileHumanizer : ITelephonyProfileHumanizer
    {
        public IDictionary<string, string> Humanize(IDictionary<string, string> rawTelephonyProfileFields)
        {
            if (rawTelephonyProfileFields == null)
                throw new ArgumentNullException(nameof(rawTelephonyProfileFields));

            // TRICK: profile name always exists
            var nodes = rawTelephonyProfileFields.Where(x => x.Key != "profile-name");

            if (rawTelephonyProfileFields.Any(x => x.Key.StartsWith("x-tel-meetingone-")))
                return HumanizeMeetingOne(rawTelephonyProfileFields);
            else if (rawTelephonyProfileFields.Any(x => x.Key.StartsWith("x-tel-arkadin-")))
                return HumanizeArkadin(rawTelephonyProfileFields);

            return new Dictionary<string, string>();
        }

        private IDictionary<string, string> HumanizeMeetingOne(IDictionary<string, string> raw)
        {
            var result = new Dictionary<string, string>();
            result.Add("Conference Room Number", raw["x-tel-meetingone-conference-id"]);
            result.Add("Host Access Code", raw["x-tel-meetingone-host-pin"]);
            return result;
        }

        private IDictionary<string, string> HumanizeArkadin(IDictionary<string, string> raw)
        {
            var result = new Dictionary<string, string>();
            result.Add("Web login", raw["x-tel-meetingone-arkadin-id"]);
            result.Add("Moderator pin code", raw["x-tel-arkadin-moderator-code"]);
            result.Add("Toll access number", raw["x-tel-arkadin-conference-number"]);
            result.Add("Toll free access number", raw["x-tel-arkadin-conference-number-free"]);
            result.Add("SIP access number", raw["x-tel-arkadin-conference-number-uvline"]);
            return result;
        }

    }

}
