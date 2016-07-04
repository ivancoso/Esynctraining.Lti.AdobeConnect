using System;
using System.Configuration;

namespace EdugameCloud.Lti.Telephony.Configuration
{
    public class MeetingOneConfigurationSection : ConfigurationSection
    {
        private static MeetingOneConfigurationSection current;


        /// <summary>
        /// Gets the current MeetingOne configuration section. if it is not yet loaded in memory, it loads it into memory
        /// </summary>
        public static MeetingOneConfigurationSection Current
        {
            get
            {
                if (current == null)
                {
                    try
                    {
                        object settings = ConfigurationManager.GetSection("meetingOne");
                        if (settings != null)
                        {
                            current = settings as MeetingOneConfigurationSection;
                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO: add to log!!
                    }
                }

                return current;
            }
        }


        [ConfigurationProperty("apiUrl", IsRequired = false)]
        public string ApiUrl
        {
            get { return (string)this["apiUrl"]; }
            set { this["apiUrl"] = value; }
        }

    }

}
