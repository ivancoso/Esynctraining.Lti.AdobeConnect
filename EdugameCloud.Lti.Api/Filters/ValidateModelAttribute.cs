using System;
using System.Linq;
using System.Runtime.Serialization;
using Esynctraining.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace EdugameCloud.Lti.Api.Filters
{
    // http://www.jerriepelser.com/blog/validation-response-aspnet-core-webapi/
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        // TODO: move to esync.core??
        [DataContract]
        public class ValidationError
        {
            [DataMember]
            public string Field { get; }

            [DataMember]
            public string Message { get; }


            public ValidationError(string field, string message)
            {
                Field = field != string.Empty ? field : null;
                Message = message;
            }

        }

        // TODO: move to esync.core??
        [DataContract]
        public class ValidationErrorModel : OperationResult
        {
            [DataMember]
            public ValidationError[] Errors { get; }


            public ValidationErrorModel(ModelStateDictionary modelState, bool isDevelopment)
            {
                if (modelState == null)
                    throw new ArgumentNullException(nameof(modelState));

                IsSuccess = false;
                Message = "Validation Failed";

                if (isDevelopment)
                Errors = modelState.Keys
                        .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                        .ToArray();
            }

        }
        
        private readonly ILogger _logger;
        private readonly bool _isDevelopment;


        public ValidateModelAttribute(ILoggerFactory loggerFactory, bool isDevelopment)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _logger = loggerFactory.CreateLogger("ModelValidation");
            _isDevelopment = isDevelopment;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var validationErrorModel = new ValidationErrorModel(context.ModelState, _isDevelopment);

                _logger.LogError("ModelValidation", validationErrorModel);

                context.Result = new ObjectResult(validationErrorModel);
            }

        }

    }

}
