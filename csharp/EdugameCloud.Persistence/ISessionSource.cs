namespace EdugameCloud.Persistence
{
    using System;
    using NHibernate;

    /// <summary>
    /// The SessionSource interface.
    /// </summary>
    public interface ISessionSource : IDisposable
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        ISession Session { get; }

    }

}