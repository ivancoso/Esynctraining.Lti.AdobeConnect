namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The theme DTO.
    /// </summary>
    [DataContract]
    public class ThemeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ThemeDTO" /> class.
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
            this.dateCreated = tm.DateCreated.ConvertToUnixTimestamp();
            this.dateModified = tm.DateModified.ConvertToUnixTimestamp();
            this.isActive = tm.IsActive;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember]
        public int? createdBy { get; set; }

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        [DataMember]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the date modified.
        /// </summary>
        [DataMember]
        public double dateModified { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        [DataMember]
        public bool? isActive { get; set; }

        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        [DataMember]
        public int? modifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the theme id.
        /// </summary>
        [DataMember]
        public int themeId { get; set; }

        /// <summary>
        /// Gets or sets the theme name.
        /// </summary>
        [DataMember]
        public string themeName { get; set; }

        #endregion
    }
}