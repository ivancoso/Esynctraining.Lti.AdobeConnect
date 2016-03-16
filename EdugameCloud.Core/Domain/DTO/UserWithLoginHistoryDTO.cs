namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Extensions;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The user DTO.
    /// </summary>
    [DataContract]
    [KnownType(typeof(UserDTO))]
    public class UserWithLoginHistoryDTO : UserDTO
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserWithLoginHistoryDTO"/> class.
        /// </summary>
        public UserWithLoginHistoryDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserWithLoginHistoryDTO"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="latest">
        /// The latest.
        /// </param>
        public UserWithLoginHistoryDTO(User user, UserLoginHistory latest) : base(user)
        {
            this.lastLogin = latest.Return(x => x.DateCreated, DateTime.Now).ConvertToUnixTimestamp();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="UserWithLoginHistoryDTO"/> class.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="latest">
        /// The latest.
        /// </param>
        public UserWithLoginHistoryDTO(User user, DateTime? latest)
            : base(user)
        {
            this.lastLogin = (latest ?? DateTime.Now).ConvertToUnixTimestamp();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the last login.
        /// </summary>
        [DataMember]
        public double lastLogin { get; set; }

        #endregion
    }
}