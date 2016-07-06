using System.Collections.Generic;
using System.Runtime.Serialization;
using EdugameCloud.Lti.Core.Domain.Entities;
using Esynctraining.AdobeConnect;

namespace EdugameCloud.Lti.DTO
{
    [DataContract]
    public class LtiViewModelDto
    {
        [DataMember(Name = "version")]
        public string LtiVersion { get; set; }

        [DataMember(Name = "currentUserName")]
        public string CurrentUserName { get; set; }


        [DataMember(Name = "acSettings")]
        public ACDetailsDTO AcSettings { get; set; }

        [DataMember(Name = "acRoles")]
        public AcRole[] AcRoles
        {
            get
            {
                return new AcRole[] { AcRole.Host, AcRole.Presenter, AcRole.Participant };
            }
            set { }
        }

        [DataMember(Name = "lmsSettings")]
        public LicenceSettingsDto LicenceSettings { get; set; }

        [DataMember(Name = "meetings")]
        public IEnumerable<MeetingDTO> Meetings { get; set; }

        [DataMember(Name = "seminars")]
        public IEnumerable<SeminarLicenseDto> Seminars { get; set; }

        [DataMember(Name = "seminarsMessage")]
        public string SeminarsMessage { get; set; }


        [DataMember(Name = "isTeacher")]
        public bool IsTeacher { get; set; }

        [DataMember(Name = "connectServer")]
        public string ConnectServer { get; set; }

        [DataMember(Name = "lmsProviderName")]
        public string LmsProviderName { get; set; }

        [DataMember(Name = "userGuideLink")]
        public string UserGuideLink { get; set; }

        [DataMember(Name = "courseMeetingsEnabled")]
        public bool CourseMeetingsEnabled { get; set; }

        [DataMember(Name = "studyGroupsEnabled")]
        public bool StudyGroupsEnabled { get; set; }

        [DataMember(Name = "syncUsersCountLimit")]
        public int SyncUsersCountLimit { get; set; }

    }

}
