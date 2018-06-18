namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(FileDTO))]
    public sealed class QuestionFromStoredProcedureDTO
    {
        [DataMember]
        public string correctMessage { get; set; }

        [DataMember]
        public string correctReference { get; set; }

        [DataMember]
        public decimal? totalWeightBucket { get; set; }

        [DataMember]
        public int? weightBucketType { get; set; }

        [DataMember]
        public string restrictions { get; set; }

        [DataMember]
        public bool? allowOther { get; set; }

        [DataMember]
        public bool isMandatory { get; set; }

        [DataMember]
        public int? height { get; set; }

        [DataMember]
        public string hint { get; set; }

        [DataMember]
        public Guid? imageId { get; set; }

        [DataMember]
        public FileDTO imageVO { get; set; }

        [DataMember]
        public string incorrectMessage { get; set; }

        [DataMember]
        public string instruction { get; set; }

        [DataMember]
        public string question { get; set; }

        [DataMember]
        public string htmlText { get; set; }

        [DataMember]
        public int questionId { get; set; }

        [DataMember]
        public int questionOrder { get; set; }

        [DataMember]
        public int? pageNumber { get; set; }

        [DataMember]
        public int questionTypeId { get; set; }

        [DataMember]
        public int scoreValue { get; set; }

        [DataMember]
        public int? width { get; set; }

        [DataMember]
        public int? x { get; set; }

        [DataMember]
        public int? y { get; set; }

        [DataMember]
        public bool? randomizeAnswers { get; set; }

        [DataMember]
        public bool? isAlwaysRateDropdown { get; set; }

        [DataMember]
        public int rows { get; set; }

        /// <summary>
        /// TODO: make different types for single and multiple choice questions, remove this property
        /// Currently is needed for Moodle (single choice which can have multiple correct answers)
        /// AA: added protected set as when WCF service is consumer as web service it throughs an error - http://stackoverflow.com/questions/2323277/wcf-chokes-on-properties-with-no-set-any-workaround
        /// </summary>
        [DataMember]
        public bool isMultipleChoice
        {
            get { return restrictions != null && restrictions.Contains("multi_choice"); }
            protected set { }
        }

    }

}