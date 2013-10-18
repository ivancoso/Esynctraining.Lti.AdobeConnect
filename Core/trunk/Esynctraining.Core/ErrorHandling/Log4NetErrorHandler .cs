namespace Esynctraining.Core.ErrorHandling
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using log4net.Core;

    using WcfRestContrib.ServiceModel.Web.Exceptions;

    using ILogger = Castle.Core.Logging.ILogger;

    /// <summary>
    /// The log 4 net error handler.
    /// </summary>
    public class Log4NetErrorHandler : System.ServiceModel.Dispatcher.IErrorHandler
    {
        #region Constants and Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger = IoC.Resolve<ILogger>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            this.logger.Error(message + errorCode, e);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void Error(string message, Exception e)
        {
            this.logger.Error(message, e);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Error(string message)
        {
            this.logger.Error(message);
        }

        /// <summary>
        /// The provide fault.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message msg)
        {
            var resultedError = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, "Error", "Server error. Try again later", error.ToString());
            if (error is WebException)
            {
                resultedError = new Error { errorMessage = error.Message, errorCode = (int)(error as WebException).Status, errorDetail = error.ToString(), errorType = "Error" };
            }

            msg = Message.CreateMessage(version, msg.With(x => x.Headers).With(x => x.Action), new ServiceResponse { error = resultedError, status = Errors.CODE_RESULTTYPE_ERROR }, new DataContractSerializer(typeof(ServiceResponse)));
        }

        /// <summary>
        /// The handle error.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HandleError(Exception exception)
        {
            this.logger.Error("Unhandled WCF exception", exception);
            return false;
        }

        #endregion
    }
}