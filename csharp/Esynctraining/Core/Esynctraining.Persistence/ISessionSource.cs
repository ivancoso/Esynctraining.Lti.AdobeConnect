namespace Esynctraining.Persistence
{
    using System;
    using global::NHibernate;

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