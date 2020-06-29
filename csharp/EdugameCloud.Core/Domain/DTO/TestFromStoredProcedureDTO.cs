namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The test item DTO.
    /// </summary>
    [DataContract]
    public class TestFromStoredProcedureDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFromStoredProcedureDTO"/> class.
        /// </summary>
        public TestFromStoredProcedureDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFromStoredProcedureDTO"/> class.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        public TestFromStoredProcedureDTO(TestFromStoredProcedureExDTO dto)
        {
            this.testId = dto.testId;
            this.testName = dto.testName;
            this.categoryName = dto.categoryName;
            this.createdBy = dto.createdBy;
            this.createdByLastName = dto.createdByLastName;
            this.createdByName = dto.createdByName;
            this.dateModified = dto.dateModified.ConvertToUnixTimestamp();
            this.firstName = dto.firstName;
            this.lastName = dto.lastName;
            this.description = dto.description;
            this.timeLimit = dto.timeLimit;
            this.passingScore = dto.passingScore;
            this.scoreFormat = dto.scoreFormat;
            this.instructionDescription = dto.instructionDescription;
            this.instructionTitle = dto.instructionTitle;
            this.subModuleCategoryId = dto.subModuleCategoryId;
            this.subModuleItemId = dto.subModuleItemId;
            this.userId = dto.userId;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int createdBy { get; set; }

        /// <summary>
        /// Gets or sets the created by last name.
        /// </summary>
        [DataMember]
        public string createdByLastName { get; set; }

        /// <summary>
        /// Gets or sets the created by name.
        /// </summary>
        [DataMember]
        public string createdByName { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the instruction details.
        /// </summary>
        [DataMember]
        public string instructionDescription { get; set; }

        /// <summary>
        /// Gets or sets the instruction title.
        /// </summary>
        [DataMember]
        public string instructionTitle { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the passing score.
        /// </summary>
        [DataMember]
        public decimal? passingScore { get; set; }

        /// <summary>
        /// Gets or sets the score format.
        /// </summary>
        [DataMember]
        public string scoreFormat { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the test id.
        /// </summary>
        [DataMember]
        public int testId { get; set; }

        /// <summary>
        /// Gets or sets the test name.
        /// </summary>
        [DataMember]
        public string testName { get; set; }

        /// <summary>
        /// Gets or sets the time limit.
        /// </summary>
        [DataMember]
        public int? timeLimit { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        #endregion
    }
}