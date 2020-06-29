using System;
using Esynctraining.Core.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace EdugameCloud.Lti.Api.Core.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly IJsonSerializer _errorSerializer;
        private readonly bool _showValidationErrors;

        public ValidateModelAttribute(ILoggerFactory loggerFactory, IJsonSerializer errorSerializer,
            bool showValidationErrors)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            this._logger = loggerFactory.CreateLogger("ModelValidation");
            IJsonSerializer jsonSerializer = errorSerializer;
            if (jsonSerializer == null)
                throw new ArgumentNullException(nameof(errorSerializer));
            this._errorSerializer = jsonSerializer;
            this._showValidationErrors = showValidationErrors;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;
            Esynctraining.AspNetCore.Filters.ValidateModelAttribute.ValidationErrorModel validationErrorModel =
                new Esynctraining.AspNetCore.Filters.ValidateModelAttribute.ValidationErrorModel(context.ModelState);
            this._logger.LogError("ModelValidation. {0}.",
                (object)this._errorSerializer.JsonSerialize<Esynctraining.AspNetCore.Filters.ValidateModelAttribute.ValidationErrorModel>(
                    validationErrorModel));
            if (!this._showValidationErrors)
                validationErrorModel.Errors = (Esynctraining.AspNetCore.Filters.ValidateModelAttribute.ValidationError[])null;
            else if (validationErrorModel.Errors.Length >= 1) //todo: handle messages on client
                validationErrorModel.Message += $" {validationErrorModel.Errors[0].Message}";
            context.Result = (IActionResult)new ObjectResult((object)validationErrorModel);
        }
    }
}