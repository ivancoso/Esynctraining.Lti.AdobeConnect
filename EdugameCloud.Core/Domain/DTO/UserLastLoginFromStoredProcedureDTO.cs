namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The user last login DTO.
    /// </summary>
    [DataContract]
    public class UserLastLoginFromStoredProcedureDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the login date.
        /// </summary>
        [DataMember]
        public virtual DateTime loginDate { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public virtual int userId { get; set; }

        #endregion
    }
}