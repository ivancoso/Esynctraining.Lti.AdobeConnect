using System;
using System.Collections.Generic;

namespace Esynctraining.Lti.Zoom.Api.Dto
{
    public class LmsLicenseDto
    {
        public int Id { get; set; }
        public Guid ConsumerKey { get; set; }
        public Guid SharedSecret { get; set; }
        public int LmsProviderId { get; set; } //todo: enum
        public string Domain { get; set; }
        public Dictionary<string, object> Settings { get; set; }

        public T GetSetting<T>(string settingName)
        {
            return (Settings.ContainsKey(settingName))
                ? (T)Convert.ChangeType(Settings[settingName], typeof(T)) // assuming that we convert to primitive type
                : default(T);
        }

        public T GetSetting<T>(string settingName, T defaultValue)
        {
            return (Settings.ContainsKey(settingName))
                ? (T)Convert.ChangeType(Settings[settingName], typeof(T)) // assuming that we convert to primitive type
                : defaultValue;
        }
    }
}
