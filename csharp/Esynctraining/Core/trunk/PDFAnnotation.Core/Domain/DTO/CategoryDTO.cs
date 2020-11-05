namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The category
    /// </summary>
    [DataContract]
    [KnownType(typeof(TopicDTO))]
    public class CategoryDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryDTO"/> class.
        /// </summary>
        public CategoryDTO()
        {
            this.categoryContacts = new List<int>();
            this.topics = new List<TopicDTO>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryDTO"/> class.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <param name="categoryContacts">
        /// The category Contacts.
        /// </param>
        /// <param name="topics">
        /// The topics.
        /// </param>
        public CategoryDTO(
            Category category,
            IEnumerable<int> categoryContacts = null,
            IEnumerable<Topic> topics = null)
            : this()
        {
            this.categoryId = category.Id;
            this.rbCaseId = category.RBCaseId;
            this.categoryName = category.CategoryName;
            this.details = category.Details;
            this.companyId = category.Company.With(x => x.Id);
            this.isFileNumbersAutoIncremented = category.IsFileNumbersAutoIncremented;
            this.exhibitsCount = category.Files.ToList().Count(x => x.FileNumber.HasValue);

            if (categoryContacts != null)
            {
                this.categoryContacts = categoryContacts.ToList();
            }

            if (topics == null)
            {
                topics = category.Topics.ToList();
            }

            this.topics = topics.Select(x => new TopicDTO(x)).ToList();
            var topicsFilesCount = this.topics.Any() ? this.topics.Sum(x => x.exhibitsCount) : 0;
            this.exhibitsCount += topicsFilesCount;
        }

        #region Public Properties

        /// <summary>
        ///     Gets or sets the firm id.
        /// </summary>
        [DataMember]
        public List<int> categoryContacts { get; set; }

        /// <summary>
        ///     Gets or sets the category id.
        /// </summary>
        [DataMember]
        public int? rbCaseId { get; set; }

        /// <summary>
        ///     Gets or sets the category id.
        /// </summary>
        [DataMember]
        public int categoryId { get; set; }

        /// <summary>
        ///     Gets or sets the company id.
        /// </summary>
        [DataMember]
        public int companyId { get; set; }

        /// <summary>
        ///     Gets or sets topcs.
        /// </summary>
        [DataMember]
        public List<TopicDTO> topics { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is exhibits numbers auto incremented.
        /// </summary>
        [DataMember]
        public bool isFileNumbersAutoIncremented { get; set; }

        /// <summary>
        ///     Gets or sets the company name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        ///     Gets or sets the details.
        /// </summary>
        [DataMember]
        public string details { get; set; }

        /// <summary>
        ///     Gets or sets exhibits count
        /// </summary>
        [DataMember]
        public int exhibitsCount { get; set; }

        #endregion
    }
}