namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The crossword item DTO.
    /// </summary>
    [DataContract]
    public class CrosswordDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the applet item id.
        /// </summary>
        [DataMember]
        public int appletItemId { get; set; }

        /// <summary>
        /// Gets or sets the applet name.
        /// </summary>
        [DataMember]
        public string appletName { get; set; }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [DataMember]
        public string categoryName { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int createdBy { get; set; }

        /// <summary>
        /// Gets or sets the created by last name.
        /// </summary>
        [DataMember]
        public string createdByLastName { get; set; }

        /// <summary>
        /// Gets or sets the created by name.
        /// </summary>
        [DataMember]
        public string createdByName { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified
        {
            get
            {
                return this.dateModifiedData.ConvertToUnixTimestamp();
            }

            // ReSharper disable once ValueParameterNotUsed
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [IgnoreDataMember]
        public DateTime dateModifiedData { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [DataMember]
        public string firstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [DataMember]
        public string lastName { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the sub module item id.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public int userId { get; set; }

        #endregion
    }
}