namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The test DTO.
    /// </summary>
    [DataContract]
    public class TestDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDTO"/> class.
        /// </summary>
        public TestDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDTO"/> class.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public TestDTO(Test result)
        {
            this.testId = result.Id;
            this.subModuleItemId = result.SubModuleItem.Return(x => x.Id, (int?)null);
            this.scoreTypeId = result.ScoreType.Return(x => x.Id, (int?)null);
            this.instructionDescription = result.InstructionDescription;
            this.instructionTitle = result.InstructionTitle;
            this.description = result.Description;
            this.testName = result.TestName;
            this.scoreFormat = result.ScoreFormat;
            this.timeLimit = result.TimeLimit;
            this.passingScore = result.PassingScore;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the time limit.
        /// </summary>
        [DataMember]
        public int? timeLimit { get; set; }

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
        /// Gets or sets the instruction description.
        /// </summary>
        [DataMember]
        public string instructionDescription { get; set; }

        /// <summary>
        /// Gets or sets the instruction title.
        /// </summary>
        [DataMember]
        public string instructionTitle { get; set; }

        /// <summary>
        /// Gets or sets the instruction title.
        /// </summary>
        [DataMember]
        public string description { get; set; }

        /// <summary>
        /// Gets or sets the test name.
        /// </summary>
        [DataMember]
        public string testName { get; set; }

        /// <summary>
        /// Gets or sets the score type.
        /// </summary>
        [DataMember]
        public int? scoreTypeId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int? subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public int testId { get; set; }

        #endregion
    }
}