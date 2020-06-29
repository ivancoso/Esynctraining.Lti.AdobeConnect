namespace EdugameCloud.Core.Domain.Entities
{
    /// <summary>
    /// The user status.
    /// </summary>
    public enum UserStatus : int
    {
        Inactive = 0,

        Active = 1,

        Deleted = 2,

        Activating = 3
    }
}