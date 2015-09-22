namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The question.
    /// </summary>
    public class Question : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the correct message.
        /// </summary>
        public virtual string CorrectMessage { get; set; }

        /// <summary>
        /// Gets or sets the correct reference.
        /// </summary>
        public virtual string CorrectReference { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        /// Gets or sets the hint.
        /// </summary>
        public virtual string Hint { get; set; }

        /// <summary>
        /// Gets or sets the File.
        /// </summary>
        public virtual File Image { get; set; }

        /// <summary>
        /// Gets or sets the incorrect message.
        /// </summary>
        public virtual string IncorrectMessage { get; set; }

        /// <summary>
        /// Gets or sets the instruction.
        /// </summary>
        public virtual string Instruction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the question name.
        /// </summary>
        public virtual string QuestionName { get; set; }

        /// <summary>
        /// Gets or sets the question order.
        /// </summary>
        public virtual int QuestionOrder { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public virtual QuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets the score value.
        /// </summary>
        public virtual int ScoreValue { get; set; }

        /// <summary>
        /// Gets or sets the LMS question id.
        /// </summary>
        public virtual int? LmsQuestionId { get; set; }

        /// <summary>
        /// Gets or sets the LMS provider
        /// </summary>
        public virtual int? LmsProviderId { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether randomize answers.
        /// </summary>
        public virtual bool? RandomizeAnswers { get; set; }

        /// <summary>
        /// Gets or sets the quiz question results.
        /// </summary>
        public virtual IList<QuizQuestionResult> QuizQuestionResults { get; protected set; }

        /// <summary>
        /// Gets or sets the quiz question results.
        /// </summary>
        public virtual IList<SurveyQuestionResult> SurveyQuestionResults { get; protected set; }

        /// <summary>
        /// Gets or sets the quiz question results.
        /// </summary>
        public virtual IList<TestQuestionResult> TestQuestionResults { get; protected set; }

        /// <summary>
        /// Gets or sets the distractors.
        /// </summary>
        public virtual IList<Distractor> Distractors { get; protected set; }

        /// <summary>
        /// Gets or sets the questions for like.
        /// </summary>
        public virtual IList<QuestionForLikert> LikertQuestions { get; protected set; }

        /// <summary>
        /// Gets or sets the questions for rate.
        /// </summary>
        public virtual IList<QuestionForRate> RateQuestions { get; protected set; }

        /// <summary>
        /// Gets or sets the questions for true false questions.
        /// </summary>
        public virtual IList<QuestionForTrueFalse> TrueFalseQuestions { get; protected set; }

        /// <summary>
        /// Gets or sets the questions for open answer.
        /// </summary>
        public virtual IList<QuestionForOpenAnswer> OpenAnswerQuestions { get; protected set; }

        /// <summary>
        /// Gets or sets the questions for single multiple choice.
        /// </summary>
        public virtual IList<QuestionForSingleMultipleChoice> SingleMultipleChoiceQuestions { get; protected set; }

        /// <summary>
        /// Gets or sets the questions for single multiple choice.
        /// </summary>
        public virtual IList<QuestionForWeightBucket> WeightBucketQuestions { get; protected set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        #endregion

        public Question()
        { 
            QuizQuestionResults = new List<QuizQuestionResult>();
            SurveyQuestionResults = new List<SurveyQuestionResult>();
            TestQuestionResults = new List<TestQuestionResult>();
            LikertQuestions = new List<QuestionForLikert>();
            RateQuestions = new List<QuestionForRate>();
            TrueFalseQuestions = new List<QuestionForTrueFalse>();
            OpenAnswerQuestions = new List<QuestionForOpenAnswer>();
            WeightBucketQuestions = new List<QuestionForWeightBucket>();
            SingleMultipleChoiceQuestions = new List<QuestionForSingleMultipleChoice>();
            Distractors = new List<Distractor>();
        }

    }

}