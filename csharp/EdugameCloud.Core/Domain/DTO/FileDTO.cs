namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The File DTO.
    /// </summary>
    [DataContract]
    public class FileDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDTO"/> class.
        /// </summary>
        public FileDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDTO"/> class.
        /// </summary>
        /// <param name="file">
        /// The File.
        /// </param>
        public FileDTO(File file)
        {
            this.fileId = file.Id;
            this.dateCreated = file.DateCreated.ConvertToUnixTimestamp();
            this.height = file.Height;
            this.width = file.Width;
            this.x = file.X;
            this.y = file.Y;
            this.fileName = file.FileName;
            this.createdBy = file.CreatedBy.With(x => x.Id);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        [DataMember(IsRequired = false)]
        public double dateCreated { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        [DataMember(IsRequired = false)]
        public int? height { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(IsRequired = false)]
        public Guid fileId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember(IsRequired = true)]
        public string fileName { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [DataMember(IsRequired = false)]
        public int? width { get; set; }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        [DataMember(IsRequired = false)]
        public int? x { get; set; }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        [DataMember(IsRequired = false)]
        public int? y { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        [DataMember(IsRequired = false)]
        public int? createdBy { get; set; }

        #endregion
    }
}