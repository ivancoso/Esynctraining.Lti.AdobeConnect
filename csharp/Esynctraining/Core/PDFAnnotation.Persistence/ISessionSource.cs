namespace PDFAnnotation.Persistence
{
    using System;

    using NHibernate;

    /// <summary>
    /// The SessionSource interface.
    /// </summary>
    public interface ISessionSource : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Gets the session.
        /// </summary>
        ISession Session { get; }

        #endregion
    }
}