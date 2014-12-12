namespace EdugameCloud.Core.Domain.Entities
{
    using System;

    using Esynctraining.Core.Domain.Entities;

    using Iesi.Collections.Generic;

    /// <summary>
    ///     The question.
    /// </summary>
    public class Question : Entity
    {
        #region Fields

        /// <summary>
        /// The quiz question results.
        /// </summary>
        private ISet<QuizQuestionResult> quizQuestionResults = new HashedSet<QuizQuestionResult>();

        /// <summary>
        /// The survey question results.
        /// </summary>
        private ISet<SurveyQuestionResult> surveyQuestionResults = new HashedSet<SurveyQuestionResult>();

        /// <summary>
        /// The test question results.
        /// </summary>
        private ISet<TestQuestionResult> testQuestionResults = new HashedSet<TestQuestionResult>();

        /// <summary>
        /// The like questions.
        /// </summary>
        private ISet<QuestionForLikert> likertQuestions = new HashedSet<QuestionForLikert>();

        /// <summary>
        /// The rate questions.
        /// </summary>
        private ISet<QuestionForRate> rateQuestions = new HashedSet<QuestionForRate>();

        /// <summary>
        /// The true false questions.
        /// </summary>
        private ISet<QuestionForTrueFalse> trueFalseQuestions = new HashedSet<QuestionForTrueFalse>();

        /// <summary>
        /// The open answer questions.
        /// </summary>
        private ISet<QuestionForOpenAnswer> openAnswerQuestions = new HashedSet<QuestionForOpenAnswer>();

        /// <summary>
        /// The weight questions.
        /// </summary>
        private ISet<QuestionForWeightBucket> weightQuestions = new HashedSet<QuestionForWeightBucket>();

        /// <summary>
        /// The single multiple choice questions.
        /// </summary>
        private ISet<QuestionForSingleMultipleChoice> singleMultipleChoiceQuestions = new HashedSet<QuestionForSingleMultipleChoice>();

        /// <summary>
        /// The distractors.
        /// </summary>
        private ISet<Distractor> distractors = new HashedSet<Distractor>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the correct message.
        /// </summary>
        public virtual string CorrectMessage { get; set; }

        /// <summary>
        ///     Gets or sets the correct reference.
        /// </summary>
        public virtual string CorrectReference { get; set; }

        /// <summary>
        ///     Gets or sets the created by.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the date modified.
        /// </summary>
        public virtual DateTime DateModified { get; set; }

        /// <summary>
        ///     Gets or sets the hint.
        /// </summary>
        public virtual string Hint { get; set; }

        /// <summary>
        ///     Gets or sets the File.
        /// </summary>
        public virtual File Image { get; set; }

        /// <summary>
        ///     Gets or sets the incorrect message.
        /// </summary>
        public virtual string IncorrectMessage { get; set; }

        /// <summary>
        ///     Gets or sets the instruction.
        /// </summary>
        public virtual string Instruction { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is active.
        /// </summary>
        public virtual bool? IsActive { get; set; }

        /// <summary>
        ///     Gets or sets the modified by.
        /// </summary>
        public virtual User ModifiedBy { get; set; }

        /// <summary>
        ///     Gets or sets the question name.
        /// </summary>
        public virtual string QuestionName { get; set; }

        /// <summary>
        ///     Gets or sets the question order.
        /// </summary>
        public virtual int QuestionOrder { get; set; }

        /// <summary>
        ///     Gets or sets the question type.
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
        /// Gets or sets the value indicating whether is single.
        /// </summary>
        public virtual bool? IsMoodleSingle { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether randomize answers.
        /// </summary>
        public virtual bool? RandomizeAnswers { get; set; }

        /// <summary>
        /// Gets or sets the quiz question results.
        /// </summary>
        public virtual ISet<QuizQuestionResult> QuizQuestionResults
        {
            get
            {
                return this.quizQuestionResults;
            }

            set
            {
                this.quizQuestionResults = value;
            }
        }

        /// <summary>
        /// Gets or sets the quiz question results.
        /// </summary>
        public virtual ISet<SurveyQuestionResult> SurveyQuestionResults
        {
            get
            {
                return this.surveyQuestionResults;
            }

            set
            {
                this.surveyQuestionResults = value;
            }
        }

        /// <summary>
        /// Gets or sets the quiz question results.
        /// </summary>
        public virtual ISet<TestQuestionResult> TestQuestionResults
        {
            get
            {
                return this.testQuestionResults;
            }

            set
            {
                this.testQuestionResults = value;
            }
        }

        /// <summary>
        /// Gets or sets the distractors.
        /// </summary>
        public virtual ISet<Distractor> Distractors
        {
            get
            {
                return this.distractors;
            }

            set
            {
                this.distractors = value;
            }
        }

        /// <summary>
        /// Gets or sets the questions for like.
        /// </summary>
        public virtual ISet<QuestionForLikert> LikertQuestions
        {
            get
            {
                return this.likertQuestions;
            }

            set
            {
                this.likertQuestions = value;
            }
        }

        /// <summary>
        /// Gets or sets the questions for rate.
        /// </summary>
        public virtual ISet<QuestionForRate> RateQuestions
        {
            get
            {
                return this.rateQuestions;
            }

            set
            {
                this.rateQuestions = value;
            }
        }

        /// <summary>
        /// Gets or sets the questions for true false questions.
        /// </summary>
        public virtual ISet<QuestionForTrueFalse> TrueFalseQuestions
        {
            get
            {
                return this.trueFalseQuestions;
            }

            set
            {
                this.trueFalseQuestions = value;
            }
        }

        /// <summary>
        /// Gets or sets the questions for open answer.
        /// </summary>
        public virtual ISet<QuestionForOpenAnswer> OpenAnswerQuestions
        {
            get
            {
                return this.openAnswerQuestions;
            }

            set
            {
                this.openAnswerQuestions = value;
            }
        }

        /// <summary>
        /// Gets or sets the questions for single multiple choice.
        /// </summary>
        public virtual ISet<QuestionForSingleMultipleChoice> SingleMultipleChoiceQuestions
        {
            get
            {
                return this.singleMultipleChoiceQuestions;
            }

            set
            {
                this.singleMultipleChoiceQuestions = value;
            }
        }

        /// <summary>
        /// Gets or sets the questions for single multiple choice.
        /// </summary>
        public virtual ISet<QuestionForWeightBucket> WeightBucketQuestions
        {
            get
            {
                return this.weightQuestions;
            }

            set
            {
                this.weightQuestions = value;
            }
        }

        /// <summary>
        ///     Gets or sets the sub module item.
        /// </summary>
        public virtual SubModuleItem SubModuleItem { get; set; }

        #endregion
    }
}