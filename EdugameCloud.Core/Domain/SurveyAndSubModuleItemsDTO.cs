namespace EdugameCloud.Core.Domain
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.DTO;

    /// <summary>
    /// The surveys and sub module items dto.
    /// </summary>
    [DataContract]
    public class SurveysAndSubModuleItemsDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the quizes.
        /// </summary>
        [DataMember]
        public List<SurveyFromStoredProcedureDTO> surveys { get; set; }

        /// <summary>
        /// Gets or sets the sub module items.
        /// </summary>
        [DataMember]
        public List<SubModuleItemDTO> subModuleItems { get; set; }

        #endregion
    }
}
