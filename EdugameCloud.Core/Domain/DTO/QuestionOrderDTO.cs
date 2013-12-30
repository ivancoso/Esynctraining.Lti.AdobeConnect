namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The question order dto.
    /// </summary>
    [DataContract]
    public class QuestionOrderDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        [DataMember]
        public int questionId { get; set; }

        /// <summary>
        /// Gets or sets the question order.
        /// </summary>
        [DataMember]
        public int questionOrder { get; set; }

        #endregion
    }
}