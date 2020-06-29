namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz SMI wrapper DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(SurveyDTO))]
    [KnownType(typeof(SubModuleItemDTO))]
    public class SurveySMIWrapperDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz DTO.
        /// </summary>
        [DataMember]
        public SurveyDTO SurveyDTO { get; set; }

        /// <summary>
        /// Gets or sets the SMI DTO.
        /// </summary>
        [DataMember]
        public SubModuleItemDTO SmiDTO { get; set; }

        #endregion
    }
}