namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The quiz result save all DTO.
    /// </summary>
    [DataContract]
    public class QuizResultSaveAllDTO
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
        public QuizResultDTO[] saved { get; set; }

        #endregion
    }
}