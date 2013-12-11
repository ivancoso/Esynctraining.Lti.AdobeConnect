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
    public class ExtendedCategoryDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCategoryDTO"/> class.
        /// </summary>
        public ExtendedCategoryDTO()
        {
            this.topics = new List<TopicDTO>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCategoryDTO"/> class.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <param name="t">
        /// The topics.
        /// </param>
        public ExtendedCategoryDTO(Category category, IEnumerable<Topic> t = null) : this()
        {
            this.categoryId = category.Id;
            this.categoryName = category.CategoryName;
            this.details = category.Details;
            this.companyId = category.Company.With(x => x.Id);
            if (t != null)
            {
                this.topics = t.Select(x => new TopicDTO(x)).ToList();
            }
        }

        #region Public Properties

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
        ///     Gets or sets the topics.
        /// </summary>
        [DataMember]
        public List<TopicDTO> topics { get; set; }

        /// <summary>
        ///     Gets or sets the state name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        ///     Gets or sets the details.
        /// </summary>
        [DataMember]
        public string details { get; set; }

        #endregion
    }
}