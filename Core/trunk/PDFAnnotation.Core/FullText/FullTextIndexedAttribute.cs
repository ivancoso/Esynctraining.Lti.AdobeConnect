namespace PDFAnnotation.Core.FullText
{
    using System;

    /// <summary>
    /// The full text indexed attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FullTextIndexedAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextIndexedAttribute"/> class.
        /// </summary>
        public FullTextIndexedAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextIndexedAttribute"/> class.
        /// </summary>
        /// <param name="indexPriority">
        /// The index priority.
        /// </param>
        public FullTextIndexedAttribute(int indexPriority)
        {
            this.IndexPriority = indexPriority;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the index priority.
        /// </summary>
        public int IndexPriority { get; set; }

        #endregion
    }
}