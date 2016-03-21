using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Esynctraining.AdobeConnect
{
    [DataContract]
    public class ACDetailsDTO
    {
        #region TimeZones

        public static readonly Dictionary<int, string> TimeZones = new Dictionary<int, string>
        {
            { 0, "Dateline Standard Time" },
            { 1, "Samoa Standard Time" },
            { 2, "Hawaiian Standard Time" },
            { 3, "Alaskan Standard Time" },
            { 4, "Pacific Standard Time" },
            { 10, "Mountain Standard Time" },
            { 13, "Mexico Standard Time 2" },
            { 15, "U.S. Mountain Standard Time" },
            { 20, "Central Standard Time" },
            { 25, "Canada Central Standard Time" },
            { 30, "Mexico Standard Time" },
            { 33, "Central America Standard Time" },
            { 35, "Eastern Standard Time" },
            { 40, "U.S. Eastern Standard Time" },
            { 45, "S.A. Pacific Standard Time" },
            { 50, "Atlantic Standard Time" },
            { 55, "S.A. Western Standard Time" },
            { 56, "Pacific S.A. Standard Time" },
            { 60, "Newfoundland and Labrador Standard Time" },
            { 65, "E. South America Standard Time" },
            { 70, "S.A. Eastern Standard Time" },
            { 73, "Greenland Standard Time" },
            { 75, "Mid-Atlantic Standard Time" },
            { 80, "Azores Standard Time" },
            { 83, "Cape Verde Standard Time" },
            { 85, "GMT Standard Time" },
            { 90, "Greenwich Standard Time" },
            { 95, "Central Europe Standard Time" },
            { 100, "Central European Standard Time" },
            { 105, "Romance Standard Time" },
            { 110, "W. Europe Standard Time" },
            { 113, "W. Central Africa Standard Time" },
            { 115, "E. Europe Standard Time" },
            { 120, "Egypt Standard Time" },
            { 125, "FLE Standard Time" },
            { 130, "GTB Standard Time" },
            { 135, "Israel Standard Time" },
            { 140, "South Africa Standard Time" },
            { 145, "Russian Standard Time" },
            { 150, "Arab Standard Time" },
            { 155, "E. Africa Standard Time" },
            { 158, "Arabic Standard Time" },
            { 160, "Iran Standard Time" },
            { 165, "Arabian Standard Time" },
            { 170, "Caucasus Standard Time" },
            { 175, "Transitional Islamic State of Afghanistan Standard Time" },
            { 180, "Ekaterinburg Standard Time" },
            { 185, "West Asia Standard Time" },
            { 190, "India Standard Time" },
            { 193, "Nepal Standard Time" },
            { 195, "Central Asia Standard Time" },
            { 200, "Sri Lanka Standard Time" },
            { 201, "N. Central Asia Standard Time" },
            { 203, "Myanmar Standard Time" },
            { 205, "S.E. Asia Standard Time" },
            { 207, "North Asia Standard Time" },
            { 210, "China Standard Time" },
            { 215, "Singapore Standard Time" },
            { 220, "Taipei Standard Time" },
            { 225, "W. Australia Standard Time" },
            { 227, "North Asia East Standard Time" },
            { 230, "Korea Standard Time" },
            { 235, "Tokyo Standard Time" },
            { 240, "Yakutsk Standard Time" },
            { 245, "A.U.S. Central Standard Time" },
            { 250, "Cen. Australia Standard Time" },
            { 255, "A.U.S. Eastern Standard Time" },
            { 260, "E. Australia Standard Time" },
            { 265, "Tasmania Standard Time" },
            { 270, "Vladivostok Standard Time" },
            { 275, "West Pacific Standard Time" },
            { 280, "Central Pacific Standard Time" },
            { 285, "Fiji Islands Standard Time" },
            { 290, "New Zealand Standard Time" },
            { 300, "Tonga Standard Time" },
        };

        #endregion TimeZones


        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "maxMeetingNameLength")]
        public int MaxMeetingNameLength { get; set; }

        [DataMember(Name = "timeZoneShiftMinutes")]
        public int TimeZoneShiftMinutes { get; set; }

        public string TimeZoneId { get; set; }

        [DataMember(Name = "passwordPolicies")]
        public ACPasswordPoliciesDTO PasswordPolicies { get; set; }

        [DataMember(Name = "customization")]
        public CustomizationDTO Customization { get; set; }


        public ACDetailsDTO()
        {
            MaxMeetingNameLength = 60;
        }


        public static string GetTimezoneId(int timeZoneId)
        {
            string msTimeZoneId = null;
            if (TimeZones.TryGetValue(timeZoneId, out msTimeZoneId))
                return msTimeZoneId;
            return null;
        }

        public TimeZoneInfo GetTimeZone()
        {
            if (string.IsNullOrWhiteSpace(TimeZoneId))
                return null;

            return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
        }


    }

}
