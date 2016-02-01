using System;
using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    //todo:should implement all common info fields
    [Serializable]
    [XmlRoot("common")]
    public class CommonInfo
    {
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
