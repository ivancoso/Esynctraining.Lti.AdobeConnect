namespace EdugameCloud.MVC.Exceptions
{
    using System;

    using Esynctraining.Core.Extensions;

    /// <summary>
    /// The access denied exception.
    /// </summary>
    public class AccessDeniedException : Exception
    {
        #region Public Properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        public override string Message
        {
            get
            {
                return "AccessDeniedException".FromResource("Errors");
            }
        }

        #endregion
    }
}