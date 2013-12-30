namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    /// The user role.
    /// </summary>
    public enum UserRoleEnum : int
    {
        /// <summary>
        /// The administrator.
        /// </summary>
        Admin = 9, 

        /// <summary>
        /// The user.
        /// </summary>
        User = 10,

        /// <summary>
        /// Any user.
        /// </summary>
        Any = 0
    }
}