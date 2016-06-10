using System.Collections.Generic;

namespace PDFAnnotation.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;
  //  using Esynctraining.Core.FullText;
    using Iesi.Collections.Generic;

    /// <summary>
    ///     The case
    /// </summary>
 //   [FullTextEnabled]
    public class Category : Entity
    {
        /// <summary>
        /// The emailHistory.
        /// </summary>
        private ISet<Topic> topics = new HashSet<Topic>();

        /// <summary>
        /// The files.
        /// </summary>
        private ISet<File> files = new HashSet<File>();

        /// <summary>
        /// The contacts.
        /// </summary>
//        private ISet<Contact> contacts = new HashSet<Contact>();

        #region Public Properties

        /// <summary>
        /// Gets or sets the RB case id.
        /// </summary>
        public virtual int? RBCaseId { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        public virtual int CompanyId { get; set; }

        /// <summary>
        ///     Gets or sets the case name.
        /// </summary>
     //   [FullTextIndexed(0)]
        public virtual string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is exhibits numbers auto incremented.
        /// </summary>
        public virtual bool IsFileNumbersAutoIncremented { get; set; }

        /// <summary>
        ///     Gets or sets the details.
        /// </summary>
     //   [FullTextIndexed(1)]
        public virtual string Details { get; set; }

        /// <summary>
        /// Gets or sets the topics.
        /// </summary>
        public virtual ISet<Topic> Topics
        {
            get
            {
                return this.topics;
            }

            set
            {
                this.topics = value;
            }
        }

        /// <summary>
        ///     Gets or sets the files.
        /// </summary>
        public virtual ISet<File> Files
        {
            get
            {
                return this.files;
            }

            set
            {
                this.files = value;
            }
        }

   #if CONTACTS_DEFINED
        /// <summary>
        ///     Gets or sets the contacts.
        /// </summary>
        public virtual ISet<Contact> Contacts
        {
            get
            {
                return this.contacts;
            }

            set
            {
                this.contacts = value;
            }
        }
#endif

#endregion
    }
}