namespace Esynctraining.Wcf.ErrorHandling
{
    using System;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Web;
    using System.Web;
    using Core;
    using DotAmf.ServiceModel.Dispatcher;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Logging;
    using Esynctraining.Core.Utils;

    internal sealed class Log4NetErrorHandler : IErrorHandler
    {
        #region Fields

        private readonly ILogger logger = IoC.Resolve<ILogger>();

        #endregion

        #region Public Properties

        private static bool IsDev
        {
            get 
            {
                if (HttpContext.Current == null)
                    return false;
                return HttpContext.Current.IsDebuggingEnabled; 
            }
        }

        #endregion

        #region Public Methods and Operators
        
        public bool HandleError(Exception exception)
        {
            logger.Error(ErrorsTexts.UnexpectedError_Subject, exception);
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message msg)
        {
            OperationContext oc = OperationContext.Current;

            if (oc == null)
                return;

            string wasCalledOn = oc.EndpointDispatcher.EndpointAddress.Uri.ToString();
            if (wasCalledOn.EndsWith("jsonp", StringComparison.OrdinalIgnoreCase))
            {
                ProvideJsonpFault(error, version, ref msg);
            }
            else if (wasCalledOn.EndsWith("amf", StringComparison.OrdinalIgnoreCase))
            {
                ProvideAmfFault(ref error, version, ref msg, oc);
            }
        }


        private static void ProvideAmfFault(ref Exception error, MessageVersion version, ref Message msg, OperationContext oc)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            // TRICK: get original stack trace here
            string stackTrace = IsDev ? error.StackTrace : string.Empty;

            if (!(error is FaultException<Error>))
            {
                string details = (IsDev || error is IUserMessageException)
                    ? error.Message
                    : ErrorsTexts.UnexpectedError;
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

        private static void ProvideJsonpFault(Exception error, MessageVersion version, ref Message msg)
        {
            var resultedError = new WcfErrorWrapper
            {
                ErrorCode = Errors.CODE_ERRORTYPE_GENERIC_ERROR,
                Message = IsDev ? error.Message : ErrorsTexts.UnexpectedError,
                StackTrace = IsDev ? error.StackTrace : string.Empty,
                FullException = error.ToString(),
                Title = ErrorsTexts.UnexpectedError_Subject,
            };

            FaultException<Error> webError = error as FaultException<Error>;
            if (webError != null)
            {
                resultedError.ErrorCode = webError.Detail.Return(x => x.errorCode, resultedError.ErrorCode);
                resultedError.Message = webError.Detail.Return(x => x.errorMessage, resultedError.Message);
                resultedError.Title = ErrorsTexts.ApplicationError;
            }
            else
            {
                IUserMessageException userMessage = error as IUserMessageException;
                if (userMessage != null)
                {
                    resultedError.Message = error.Message;
                    resultedError.Title = ErrorsTexts.ApplicationError;
                }
            }

            msg = Message.CreateMessage(version, "json", resultedError, new DataContractJsonSerializer(typeof(WcfErrorWrapper)));
            var wbf = new WebBodyFormatMessageProperty(WebContentFormat.Json);
            msg.Properties.Add(WebBodyFormatMessageProperty.Name, wbf);

            var response = WebOperationContext.Current.OutgoingResponse;
            response.ContentType = "application/json";
            response.StatusCode = HttpStatusCode.OK; 
        }

        #endregion

    }

}