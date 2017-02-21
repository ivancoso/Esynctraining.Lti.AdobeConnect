using EdugameCloud.Lti.Core.Constants;

namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;
    using EdugameCloud.Lti.Domain.Entities;

    [DataContract]
    public sealed class TelephonyDTO
    {
        public static class SupportedProfiles
        {
            public static readonly string None = "None";
            public static readonly string MeetingOne = "MeetingOne";
            public static readonly string Arkadin = "Arkadin";
        }

        public TelephonyDTO()
        {
            ActiveProfile = SupportedProfiles.None;
            MeetingOne = new TelephonyMeetingOneDTO();
            Arkadin = new TelephonyArkadinDTO();
        }

        public TelephonyDTO(LmsCompany instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            

            ActiveProfile = instance.GetSetting<string>(LmsCompanySettingNames.Telephony.ActiveProfile);

            CourseMeetingOption = instance.GetSetting<int>(LmsCompanySettingNames.Telephony.CourseMeetingOption);
            OfficeHoursOption = instance.GetSetting<int>(LmsCompanySettingNames.Telephony.OfficeHoursOption);
            StudyGroupOption = instance.GetSetting<int>(LmsCompanySettingNames.Telephony.StudyGroupOption);

            MeetingOne = new TelephonyMeetingOneDTO
            {
                OwningAccountNumber = instance.GetSetting<string>(LmsCompanySettingNames.Telephony.MeetingOne.OwningAccountNumber),
                UserName = instance.GetSetting<string>(LmsCompanySettingNames.Telephony.MeetingOne.UserName),
                SecretHashKey = instance.GetSetting<string>(LmsCompanySettingNames.Telephony.MeetingOne.SecretHashKey),
            };

            Arkadin = new TelephonyArkadinDTO
            {
                UserName = instance.GetSetting<string>(LmsCompanySettingNames.Telephony.Arkadin.UserName),
            };
        }


        [DataMember]
        public string ActiveProfile { get; set; }
        
        [DataMember]
        public int CourseMeetingOption { get; set; }
        
        [DataMember]
        public int OfficeHoursOption { get; set; }
        
        [DataMember]
        public int StudyGroupOption { get; set; }


        [DataMember]
        public TelephonyMeetingOneDTO MeetingOne { get; set; }

        [DataMember]
        public TelephonyArkadinDTO Arkadin { get; set; }

    }

}
