namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The sub module item save all DTO.
    /// </summary>
    [DataContract]
    public class SubModuleItemSaveAllDTO
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
        public SubModuleItemDTO[] saved { get; set; }

        #endregion
    }
}