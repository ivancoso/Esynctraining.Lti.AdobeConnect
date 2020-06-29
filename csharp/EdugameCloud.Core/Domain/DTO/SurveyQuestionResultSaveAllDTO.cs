namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The applet result save all DTO.
    /// </summary>
    [DataContract]
    public class SurveyQuestionResultSaveAllDTO
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
        public SurveyQuestionResultDTO[] saved { get; set; }

        #endregion
    }
}