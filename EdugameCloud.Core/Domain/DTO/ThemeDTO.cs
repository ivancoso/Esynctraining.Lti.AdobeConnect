namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The theme dto.
    /// </summary>
    [DataContract]
    public class ThemeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeDTO"/> class.
        /// </summary>
        public ThemeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeDTO"/> class.
        /// </summary>
        /// <param name="tm">
        /// The result.
        /// </param>
        public ThemeDTO(Theme tm)
        {
            this.themeId = tm.Id;
            this.themeName = tm.ThemeName;
            this.createdBy = tm.CreatedBy.Return(x => x.Id, (int?)null);
            this.modifiedBy = tm.ModifiedBy.Return(x => x.Id, (int?)null);
            this.dateCreated = tm.DateCreated;
            this.dateModified = tm.DateModified;
            this.isActive = tm.IsActive;
        }

        #endregion

        #region Public Properties

        [DataMember]
        public virtual int themeId { get; set; }

        [DataMember]
        public virtual string themeName { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public virtual int? createdBy { get; set; }

        [DataMember]
        public virtual int? modifiedBy { get; set; }

        [DataMember]
        public virtual bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public virtual DateTime dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public virtual DateTime dateModified { get; set; }

        #endregion
    }
}