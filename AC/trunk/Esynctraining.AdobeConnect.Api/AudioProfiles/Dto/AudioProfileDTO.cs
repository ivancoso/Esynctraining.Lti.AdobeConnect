using System;
using System.Runtime.Serialization;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect.Api.AudioProfiles.Dto
{
    [DataContract]
    public class AudioProfileDto
    {
        public AudioProfileDto()
        {
        }

        public AudioProfileDto(TelephonyProfile x)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            // NOTE: other properties are not in use

            //this.adaptorId = x.AdaptorId;
            //this.name = x.Name;
            this.ProfileId = x.ProfileId;
            this.ProfileName = x.ProfileName;
            //this.profileStatus = x.ProfileStatus;
            //this.providerId = x.ProviderId;
        }

        #region Public Properties

        //public string adaptorId { get; set; }

        //public string name { get; set; }

        [DataMember]
        public string ProfileId { get; set; }

        [DataMember]
        public string ProfileName { get; set; }

        //public string profileStatus { get; set; }

        //public string providerId { get; set; }

        #endregion

    }

}