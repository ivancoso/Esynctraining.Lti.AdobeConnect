namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz smi wrapper dto.
    /// </summary>
    [DataContract]
    [KnownType(typeof(SurveyDTO))]
    [KnownType(typeof(SubModuleItemDTO))]
    public class SurveySMIWrapperDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the quiz dto.
        /// </summary>
        [DataMember]
        public SurveyDTO SurveyDTO { get; set; }

        /// <summary>
        /// Gets or sets the smi dto.
        /// </summary>
        [DataMember]
        public SubModuleItemDTO SmiDTO { get; set; }

        #endregion
    }
}