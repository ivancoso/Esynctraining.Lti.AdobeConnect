namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The SN member save all DTO.
    /// </summary>
    [DataContract]
    public class SNMemberSaveAllDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the faults.
        /// </summary>
        [DataMember]
        public string[] faults { get; set; }

        /// <summary>
        /// Gets or sets the saved.
        /// </summary>
        [DataMember]
        public SNMemberDTO[] saved { get; set; }

        #endregion
    }
}