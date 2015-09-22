namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    ///     The quiz SMI wrapper DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(TestDTO))]
    [KnownType(typeof(SubModuleItemDTO))]
    public class TestSMIWrapperDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the SMI DTO.
        /// </summary>
        [DataMember]
        public SubModuleItemDTO SmiDTO { get; set; }

        /// <summary>
        /// Gets or sets the test DTO.
        /// </summary>
        [DataMember]
        public TestDTO TestDTO { get; set; }

        #endregion
    }
}