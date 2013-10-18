namespace Esynctraining.Core.Utils
{
    using System;

    /// <summary>
    ///     The procedure.
    /// </summary>
    public delegate void Proc();

    /// <summary>
    ///     The disposable action.
    /// </summary>
    public class DisposableAction : IDisposable
    {
        #region Fields

        /// <summary>
        /// The action.
        /// </summary>
        private readonly Proc action;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableAction"/> class.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Action is null exception
        /// </exception>
        public DisposableAction(Proc action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.action = action;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.action();
        }

        #endregion
    }
}