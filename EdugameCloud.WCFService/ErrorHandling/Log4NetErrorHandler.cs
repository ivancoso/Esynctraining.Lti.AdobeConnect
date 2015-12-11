namespace EdugameCloud.WCFService.ErrorHandling
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;
    using System.Web;

    using Esynctraining.Core.Logging;

    using DotAmf.ServiceModel.Dispatcher;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using Resources;
    
    [DataContract]
    public class WcfErrorWrapper
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [DataMember]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the full exception.
        /// </summary>
        [DataMember]
        public string FullException { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        [DataMember]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        #endregion
    }

    /// <summary>
    /// The log4net error handler.
    /// </summary>
    public sealed class Log4NetErrorHandler : IErrorHandler
    {
        #region Fields
        
        private readonly ILogger logger = IoC.Resolve<ILogger>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether is development environment.
        /// </summary>
        public bool IsDev
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    // TODO: ADD LOGGING??
                    return false;
                }

                return HttpContext.Current.IsDebuggingEnabled;
            }
        }

        #endregion

        #region Public Methods and Operators

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
            return true;
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
            OperationContext oc = OperationContext.Current;


            Exception originalException = error;

            if (oc != null)
            {
                string wasCalledOn = oc.EndpointDispatcher.EndpointAddress.Uri.ToString();
                if (wasCalledOn.EndsWith("jsonp", StringComparison.InvariantCultureIgnoreCase))
                {
                    var resultedError = new WcfErrorWrapper
                    {
                        ErrorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR, 
                        Message = error.Message, 
                        StackTrace = error.StackTrace, 
                        FullException = error.ToString()
                    };
                    OutgoingWebResponseContext response = WebOperationContext.Current.With(x => x.OutgoingResponse);
                    response.StatusCode = HttpStatusCode.OK;
                    response.ContentType = "application/json";

                    if (error is FaultException<Error>)
                    {
                        var webError = (FaultException<Error>)error;
                        resultedError.ErrorCode = webError.Detail.Return(x => x.errorCode, resultedError.ErrorCode);
                        resultedError.Message = webError.Detail.Return(x => x.errorMessage, resultedError.Message);
                        resultedError.Title = "Application Error";
                    }
                    else
                    {
                        resultedError.Message = this.IsDev ? error.ToString() : ErrorsTexts.UnexpectedError;
                        resultedError.Title = ErrorsTexts.UnexpectedError_Subject;
                    }

                    msg = Message.CreateMessage(
                        version, 
                        "json", 
                        resultedError, 
                        new DataContractJsonSerializer(typeof(WcfErrorWrapper)));
                    var jsonResult = new WebBodyFormatMessageProperty(WebContentFormat.Json);
                    msg.Properties.Add(WebBodyFormatMessageProperty.Name, jsonResult);
                }
                else if (wasCalledOn.EndsWith("amf", StringComparison.InvariantCultureIgnoreCase))
                {
                    string stackTrace = this.IsDev ? error.StackTrace : string.Empty;
                    if (!(error is FaultException<Error>))
                    {
                        string details = this.IsDev ? error.ToString() : ErrorsTexts.UnexpectedError;
                        var error2 =
                            new FaultException<Error>(
                                new Error(
                                    Errors.CODE_ERRORTYPE_GENERIC_ERROR, 
                                    ErrorsTexts.UnexpectedError_Subject, 
                                    details), 
                                details);
                        error = error2;
                    }

                    AmfErrorHandler.ProvideFaultExternally(oc, true, stackTrace, error, version, ref msg);
                }
            }

            this.logger.Error("Unhandled WCF exception", originalException);
        }

        #endregion

    }

}