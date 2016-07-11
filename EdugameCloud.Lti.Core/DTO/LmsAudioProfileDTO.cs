namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;
    using Esynctraining.AC.Provider.Entities;

    [DataContract]
    public class LmsAudioProfileDTO
    {
        public LmsAudioProfileDTO()
        {
        }

        public LmsAudioProfileDTO(TelephonyProfile x)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            //this.adaptorId = x.AdaptorId;
            //this.name = x.Name;
            this.profileId = x.ProfileId;
            this.profileName = x.ProfileName;
            //this.profileStatus = x.ProfileStatus;
            //this.providerId = x.ProviderId;
        }

        #region Public Properties

        //[DataMember]
        //public string adaptorId { get; set; }

        //[DataMember]
        //public string name { get; set; }

        [DataMember]
        public string profileId { get; set; }

        [DataMember]
        public string profileName { get; set; }

        //[DataMember]
        //public string profileStatus { get; set; }

        //[DataMember]
        //public string providerId { get; set; }

        #endregion

    }

}