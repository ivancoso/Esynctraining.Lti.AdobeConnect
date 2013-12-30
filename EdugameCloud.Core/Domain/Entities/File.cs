namespace EdugameCloud.Core.Domain.Entities
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;

    /// <summary>
    ///     The file.
    /// </summary>
    [DataContract]
    public class File : Entity
    {
        #region Fields

        /// <summary>
        ///     The state.
        /// </summary>
        private ImageStatus? status = ImageStatus.Created;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="File"/> class.
        /// </summary>
        public File()
        {
            this.WebOrbId = Guid.NewGuid();
            this.DateCreated = DateTime.Now;
            this.status = ImageStatus.Created;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the owner.
        /// </summary>
        public virtual User CreatedBy { get; set; }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        [DataMember(IsRequired = false)]
        public virtual DateTime DateCreated { get; set; }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        [DataMember]
        public virtual int? Height { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public virtual string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        public virtual ImageStatus? Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
            }
        }

        /// <summary>
        ///     Gets or sets the web orb id.
        /// </summary>
        [DataMember(IsRequired = true)]
        public virtual Guid? WebOrbId { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        [DataMember]
        public virtual int? Width { get; set; }

        /// <summary>
        ///     Gets or sets the x.
        /// </summary>
        [DataMember]
        public virtual int? X { get; set; }

        /// <summary>
        ///     Gets or sets the y.
        /// </summary>
        [DataMember]
        public virtual int? Y { get; set; }

        #endregion
    }
}