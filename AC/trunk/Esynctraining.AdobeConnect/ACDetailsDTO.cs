using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect
{
    [DataContract]
    public class ACDetailsDTO
    {
        public class TimeZoneMap
        {
            public TimeZoneMap(int id, string windowsTimeZoneId, int baseUtcOffset)
            {
                Id = id;
                WindowsTimeZoneId = windowsTimeZoneId;
                BaseUtcOffset = baseUtcOffset;
            }

            public int Id { get; set; }
            public string WindowsTimeZoneId { get; set; }
            public int BaseUtcOffset { get; set; }
        }

        #region TimeZones

        public static readonly List<TimeZoneMap> TimeZones = new List<TimeZoneMap>()
        {
            new TimeZoneMap(0,   "Dateline Standard Time", -12 * 60 ),
            new TimeZoneMap(1,   "Samoa Standard Time", -11 * 60 ),
            new TimeZoneMap(2,   "Hawaiian Standard Time", -10 * 60 ),
            new TimeZoneMap(3,   "Alaskan Standard Time", -9 * 60 ),
            new TimeZoneMap(4,   "Pacific Standard Time", -8 * 60 ),
            new TimeZoneMap(10,  "Mountain Standard Time", -7 * 60 ),
            new TimeZoneMap(13,  "Mexico Standard Time 2", -7 * 60 ),
            new TimeZoneMap(15,  "U.S. Mountain Standard Time", -7 * 60 ),
            new TimeZoneMap(20,  "Central Standard Time", -6 * 60 ),
            new TimeZoneMap(25,  "Canada Central Standard Time", -6 * 60 ),
            new TimeZoneMap(30,  "Mexico Standard Time", -6 * 60 ),
            new TimeZoneMap(33,  "Central America Standard Time", -6 * 60 ),
            new TimeZoneMap(35,  "Eastern Standard Time", -5 * 60 ),
            new TimeZoneMap(40,  "U.S. Eastern Standard Time", -5 * 60 ),
            new TimeZoneMap(45,  "S.A. Pacific Standard Time", -5 * 60 ),
            new TimeZoneMap(50,  "Atlantic Standard Time", -4 * 60 ),
            new TimeZoneMap(55,  "S.A. Western Standard Time", -4 * 60 ),
            new TimeZoneMap(56,  "Pacific S.A. Standard Time", -4 * 60 ),
            new TimeZoneMap(60,  "Newfoundland and Labrador Standard Time", -3 * 60 + 30 ),
            new TimeZoneMap(65,  "E. South America Standard Time", -3 * 60 ),
            new TimeZoneMap(70,  "S.A. Eastern Standard Time", -3 * 60 ),
            new TimeZoneMap(73,  "Greenland Standard Time", -3 * 60 ),
            new TimeZoneMap(75,  "Mid-Atlantic Standard Time", -2 * 60 ),
            new TimeZoneMap(80,  "Azores Standard Time", -1 * 60 ),
            new TimeZoneMap(83,  "Cape Verde Standard Time", -1 * 60 ),
            new TimeZoneMap(85,  "GMT Standard Time", 0 ),
            new TimeZoneMap(90,  "Greenwich Standard Time", 0 ),
            new TimeZoneMap(95,  "Central Europe Standard Time", 1 * 60 ),
            new TimeZoneMap(100, "Central European Standard Time", 1 * 60 ),
            new TimeZoneMap(105, "Romance Standard Time", 1 * 60 ),
            new TimeZoneMap(110, "W. Europe Standard Time", 1 * 60 ),
            new TimeZoneMap(113, "W. Central Africa Standard Time", 1 * 60 ),
            new TimeZoneMap(115, "E. Europe Standard Time", 2 * 60 ),
            new TimeZoneMap(120, "Egypt Standard Time", 2 * 60 ),
            new TimeZoneMap(125, "FLE Standard Time", 2 * 60 ),
            new TimeZoneMap(130, "GTB Standard Time", 2 * 60 ),
            new TimeZoneMap(135, "Israel Standard Time", 2 * 60 ),
            new TimeZoneMap(140, "South Africa Standard Time", 2 * 60 ),
            new TimeZoneMap(145, "Russian Standard Time", 3 * 60 ),
            new TimeZoneMap(150, "Arab Standard Time", 3 * 60 ),
            new TimeZoneMap(155, "E. Africa Standard Time", 3 * 60 ),
            new TimeZoneMap(158, "Arabic Standard Time", 3 * 60 ),
            new TimeZoneMap(160, "Iran Standard Time", 3 * 60 + 30 ),
            new TimeZoneMap(165, "Arabian Standard Time", 4 * 60 ),
            new TimeZoneMap(170, "Caucasus Standard Time", 4 * 60 ),
            new TimeZoneMap(175, "Transitional Islamic State of Afghanistan Standard Time", 4 * 60 + 30 ),
            new TimeZoneMap(180, "Ekaterinburg Standard Time", 5 * 60 ),
            new TimeZoneMap(185, "West Asia Standard Time", 5 * 60 ),
            new TimeZoneMap(190, "India Standard Time", 5 * 60 + 30 ),
            new TimeZoneMap(193, "Nepal Standard Time", 5 * 60 + 45 ),
            new TimeZoneMap(195, "Central Asia Standard Time", 6 * 60 ),
            new TimeZoneMap(200, "Sri Lanka Standard Time", 6 * 60 ),
            new TimeZoneMap(201, "N. Central Asia Standard Time", 6 * 60 ),
            new TimeZoneMap(203, "Myanmar Standard Time", 6 * 60 + 30 ),
            new TimeZoneMap(205, "S.E. Asia Standard Time", 7 * 60 ),
            new TimeZoneMap(207, "North Asia Standard Time", 7 * 60 ),
            new TimeZoneMap(210, "China Standard Time", 8 * 60 ),
            new TimeZoneMap(215, "Singapore Standard Time", 8 * 60 ),
            new TimeZoneMap(220, "Taipei Standard Time", 8 * 60 ),
            new TimeZoneMap(225, "W. Australia Standard Time", 8 * 60 ),
            new TimeZoneMap(227, "North Asia East Standard Time", 8 * 60 ),
            new TimeZoneMap(230, "Korea Standard Time", 9 * 60 ),
            new TimeZoneMap(235, "Tokyo Standard Time", 9 * 60 ),
            new TimeZoneMap(240, "Yakutsk Standard Time", 9 * 60 ),
            new TimeZoneMap(245, "A.U.S. Central Standard Time", 9 * 60 + 30 ),
            new TimeZoneMap(250, "Cen. Australia Standard Time", 9 * 60 + 30 ),
            new TimeZoneMap(255, "A.U.S. Eastern Standard Time", 10 * 60 ),
            new TimeZoneMap(260, "E. Australia Standard Time", 10 * 60 ),
            new TimeZoneMap(265, "Tasmania Standard Time", 10 * 60 ),
            new TimeZoneMap(270, "Vladivostok Standard Time", 10 * 60 ),
            new TimeZoneMap(275, "West Pacific Standard Time", 10 * 60 ),
            new TimeZoneMap(280, "Central Pacific Standard Time", 11 * 60 ),
            new TimeZoneMap(285, "Fiji Islands Standard Time", 12 * 60 ),
            new TimeZoneMap(290, "New Zealand Standard Time", 12 * 60 ),
            new TimeZoneMap(300, "Tonga Standard Time", 13 * 60 ),
        };

        #endregion TimeZones

        private TimeZoneInfo timeZoneInfo;
        private int timezoneShiftInMinutes;

        public TimeZoneInfo GetTimeZone()
        {
            return timeZoneInfo;
        }

        public void SetTimeZone(TimeZoneInfo tzInfo)
        {
            timeZoneInfo = tzInfo;
        }

        public void SetTimezoneShift(TimeZoneInfo timezone)
        {
            if (timezone != null)
            {
                timezoneShiftInMinutes = Convert.ToInt32(timezone.BaseUtcOffset.TotalMinutes);
            }

            timezoneShiftInMinutes = 0;
        }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "maxMeetingNameLength")]
        public int MaxMeetingNameLength { get; set; }

        [DataMember(Name = "timeZoneShiftMinutes")]
        public int TimeZoneShiftMinutes
        {
            get { return timezoneShiftInMinutes; }
        }

        public TimeZoneInfo TimeZoneInfo { get; set; }

        [DataMember(Name = "passwordPolicies")]
        public ACPasswordPoliciesDTO PasswordPolicies { get; set; }

        [DataMember(Name = "customization")]
        public CustomizationDTO Customization { get; set; }


        public ACDetailsDTO()
        {
            MaxMeetingNameLength = 60;
        }
    }
}
