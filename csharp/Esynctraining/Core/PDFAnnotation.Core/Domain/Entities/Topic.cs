using System.Collections.Generic;

namespace PDFAnnotation.Core.Domain.Entities
{
    using System;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.FullText;

    using Iesi.Collections.Generic;

    /// <summary>
    /// The Topic.
    /// </summary>
    [FullTextEnabled]
    public class Topic : Entity
    {

        /// <summary>
        /// The files.
        /// </summary>
        private ISet<File> files = new HashSet<File>();

        #region Public Properties

        /// <summary>
        ///     Gets or sets Contact
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        ///     Gets or sets FirstName
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        ///     Gets or sets LastName
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        ///     Gets or sets date created
        /// </summary>
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets LastName
        /// </summary>
        [FullTextIndexed(0)]
        public virtual string FullName
        {
            get
            {
                var res = this.FirstName.Return(x => x, string.Empty) + " " + this.LastName.Return(x => x, string.Empty);
                res = res.Trim();
                return res;
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
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


        #endregion
    }
}