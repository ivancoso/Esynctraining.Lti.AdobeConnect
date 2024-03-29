﻿namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.Core.Domain.Entities;

    public class QuizResult : Entity
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the AC email.
        /// This field is populated if a registered in AdobeConnect user passed the quiz either online or on a record.
        /// </summary>
        public virtual string ACEmail { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// This field is populated if a user passed the quiz on a record.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the is archive.
        /// </summary>
        public virtual bool? IsArchive { get; set; }

        /// <summary>
        /// Gets or sets the ac session.
        /// </summary>
        public virtual int ACSessionId { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the participant name.
        /// </summary>
        public virtual string ParticipantName { get; set; }

        /// <summary>
        /// Gets or sets the quiz.
        /// </summary>
        public virtual Quiz Quiz { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        public virtual IList<QuizQuestionResult> Results { get; protected set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        public virtual int Score { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public virtual DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the LMS id.
        /// </summary>
        public virtual int LmsId { get; set; }

        /// <summary>
        /// Gets or sets the LMS id.
        /// </summary>
        public virtual bool? isCompleted { get; set; }

        /// <summary>
        /// Gets or sets the LMS user parameters.
        /// </summary>
        public virtual int? LmsUserParametersId { get; set; }

        public virtual CompanyEventQuizMapping EventQuizMapping { get; set; }

        public virtual int AppInFocusTime { get; set; }

        public virtual int AppMaximizedTime { get; set; }

        public virtual Guid Guid { get; set; }

        #endregion

        public QuizResult()
        {
            Results = new List<QuizQuestionResult>();
        }


        public virtual string BuildDownloadUrl(string root)
        {
            return $"{root}/quiz-certificate/{Guid}/download";
        }

        public virtual string BuildPreviewUrl(string root)
        {
            return $"{root}/quiz-certificate/{Guid}/preview";
        }

    }

}