namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The sub module category save all DTO.
    /// </summary>
    [DataContract]
    public class SubModuleCategorySaveAllDTO
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
        public SubModuleCategoryDTO[] saved { get; set; }

        #endregion
    }
}