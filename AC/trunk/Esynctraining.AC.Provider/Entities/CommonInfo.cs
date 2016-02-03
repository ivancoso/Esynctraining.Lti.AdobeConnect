using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    //todo:should implement all common info fields
    [Serializable]
    [XmlRoot("common")]
    public class CommonInfo
    {
        // Source: http://help.adobe.com/en_US/connect/8.0/webservices/WS8d7bb3e8da6fb92f73b3823d121e63182fe-8000_SP1.html#WS5b3ccc516d4fbf351e63e3d11a171ddf77-7fa0_SP1
        private static readonly Dictionary<int, int> timeZones = new Dictionary<int, int>
        {
            { 0, -12 * 60 },
            { 1, -11 * 60 },
            { 2, -10 * 60 },
            { 3, -9 * 60 },
            { 4, -8 * 60 },
            { 10, -7 * 60 },
            { 13, -7 * 60 },
            { 15, -7 * 60 },
            { 20, -6 * 60 },
            { 25, -6 * 60 },
            { 30, -6 * 60 },
            { 33, -6 * 60 },
            { 35, -5 * 60 },
            { 40, -5 * 60 },
            { 45, -5 * 60 },
            { 50, -4 * 60 },
            { 55, -4 * 60 },
            { 56, -4 * 60 },
            { 60, -3 * 60 - 30 }, // GMT-03:30) Newfoundland
            { 65, -3 * 60 },
            { 70, -3 * 60 },
            { 73, -3 * 60 },
            { 75, -2 * 60 },
            { 80, -1 * 60 },
            { 83, -1 * 60},
            { 85, 0 },
            { 90, 0 },
            { 95, 1 * 60 },
            { 100, 1 * 60 },
            { 105, 1 * 60 },
            { 110, 1 * 60 },
            { 113, 1 * 60 },
            { 115, 2 * 60 },
            { 120, 2 * 60 },
            { 125, 2 * 60 },
            { 130, 2 * 60 },
            { 135, 2 * 60 },
            { 140, 2 * 60 },
            { 145, 3 * 60 },
            { 150, 3 * 60 },
            { 155, 3 * 60 },
            { 158, 3 * 60 },
            { 160, 3 * 60 + 30 },  //.------!!!!!!! 3.30
            { 165, 4 * 60 },
            { 170, 4 * 60 },
            { 175, 4 * 60 + 30 }, // 4.30
            { 180, 5 * 60 },
            { 185, 5 * 60 },
            { 190, 5 * 60 + 30 }, // 5.30
            { 193, 5 * 60 + 45 }, // 5.45
            { 195, 6 * 60 },
            { 200, 6 * 60 },
            { 201, 6 * 60 },
            { 203, 6 * 60 + 30 }, // 6.30
            { 205, 7 * 60 },
            { 207, 7 * 60 },
            { 210, 8 * 60 },
            { 215, 8 * 60 },
            { 220, 8 * 60 },
            { 225, 8 * 60 },
            { 227, 8 * 60 },
            { 230, 9 * 60 },
            { 235, 9 * 60 },
            { 240, 9 * 60 },
            { 245, 9 * 60 + 30 }, //9-30
            { 250, 9 * 60 + 30 }, //9-30
            { 255, 10 * 60 },
            { 260, 10 * 60 },
            { 265, 10 * 60 },
            { 270, 10 * 60 },
            { 275, 10 * 60 },
            { 280, 11 * 60 },
            { 285, 12 * 60 },
            { 290, 12 * 60 },
            { 300, 13 * 60 },
        };
        
        [XmlElement("host")]
        public string AccountUrl { get; set; }

        [XmlElement("version")]
        public string Version { get; set; }

        [XmlElement("cookie")]
        public string Cookie { get; set; }

        [XmlElement("date")]
        public DateTime Date { get; set; }

        [XmlElement("local-host")]
        public string LocalHost { get; set; }

        [XmlElement("admin-host")]
        public string AdminHost { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("mobile-app-package")]
        public string MobileAppPackage { get; set; }

        [XmlAttribute("time-zone-id")]
        public int TimeZoneId { get; set; }

        [XmlAttribute("locale")]
        public string Locale { get; set; }

        public int? AccountId { get; set; }

        public int MajorVersion
        {
            get
            {
                return GetParsedVersionValue(0);
            }
        }

        public int MinorVersion
        {
            get
            {
                return GetParsedVersionValue(1);
            }
        }

        [XmlElement("user")]
        public UserInfo User { get; set; }

        public int GetTimeZoneShiftMinutes()
        {
            int value;
            if (timeZones.TryGetValue(TimeZoneId, out value))
                return value;

            throw new InvalidOperationException(string.Format("Not supported Adobe Connect time zone. time-zone-id={0}", TimeZoneId));
        }


        private int GetParsedVersionValue(int index)
        {
            var parsedValue = 0;
            if (!String.IsNullOrEmpty(Version))
            {
                var splittedVersion = Version.Split('.');
                if (splittedVersion.Length > index)
                {
                    int.TryParse(splittedVersion[index], out parsedValue);
                }
            }

            return parsedValue;
        }

    }

}
