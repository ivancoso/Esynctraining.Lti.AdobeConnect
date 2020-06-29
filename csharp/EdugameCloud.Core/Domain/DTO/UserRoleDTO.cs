namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The user role.
    /// </summary>
    [DataContract]
    public class UserRoleDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleDTO"/> class.
        /// </summary>
        public UserRoleDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleDTO"/> class.
        /// </summary>
        /// <param name="role">
        /// The role.
        /// </param>
        public UserRoleDTO(UserRole role)
        {
            this.userRoleId = role.Id;
            this.userRole = role.UserRoleName;
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the user role id.
        /// </summary>
        [DataMember]
        public int userRoleId { get; set; }

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        [DataMember]
        public string userRole { get; set; }

        #endregion
    }
}