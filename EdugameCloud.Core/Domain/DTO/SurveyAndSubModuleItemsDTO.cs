namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The surveys and sub module items DTO.
    /// </summary>
    [DataContract]
    public class SurveysAndSubModuleItemsDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the surveys.
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
