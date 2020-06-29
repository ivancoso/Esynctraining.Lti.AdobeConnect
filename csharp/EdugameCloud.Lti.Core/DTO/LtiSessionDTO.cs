using Esynctraining.Lti.Lms.Common.Dto;

namespace EdugameCloud.Lti.DTO
{
    using System;
    using System.Runtime.Serialization;

    //todo: remove this class
    /// <summary>
    /// The LTI session DTO.
    /// </summary>
    [DataContract]
    [Serializable]
    public class LtiSessionDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the LTI param.
        /// </summary>
        [DataMember]
        public virtual LtiParamDTO LtiParam { get; set; }

        #endregion
    }
}