using System;
using System.Runtime.Serialization;
using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.Lti.Lms.Common.Constants;

namespace EdugameCloud.Lti.DTO
{
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
            

            ActiveProfile = instance.GetSetting<string>(LmsLicenseSettingNames.Telephony.ActiveProfile);

            CourseMeetingOption = instance.GetSetting<int>(LmsLicenseSettingNames.Telephony.CourseMeetingOption);
            OfficeHoursOption = instance.GetSetting<int>(LmsLicenseSettingNames.Telephony.OfficeHoursOption);
            StudyGroupOption = instance.GetSetting<int>(LmsLicenseSettingNames.Telephony.StudyGroupOption);

            MeetingOne = new TelephonyMeetingOneDTO
            {
                OwningAccountNumber = instance.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.OwningAccountNumber),
                UserName = instance.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.UserName),
                SecretHashKey = instance.GetSetting<string>(LmsLicenseSettingNames.Telephony.MeetingOne.SecretHashKey),
            };

            Arkadin = new TelephonyArkadinDTO
            {
                UserName = instance.GetSetting<string>(LmsLicenseSettingNames.Telephony.Arkadin.UserName),
            };
        }


        [DataMember(Name = "activeProfile")]
        public string ActiveProfile { get; set; }

        [DataMember(Name = "courseMeetingOption")]
        public int CourseMeetingOption { get; set; }

        [DataMember(Name = "officeHoursOption")]
        public int OfficeHoursOption { get; set; }

        [DataMember(Name = "studyGroupOption")]
        public int StudyGroupOption { get; set; }


        [DataMember(Name = "meetingOne")]
        public TelephonyMeetingOneDTO MeetingOne { get; set; }

        [DataMember(Name = "arkadin")]
        public TelephonyArkadinDTO Arkadin { get; set; }

    }

}
