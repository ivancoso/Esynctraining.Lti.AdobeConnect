using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Esynctraining.AspNetCore.Filters
{
    //[AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckModelForNullAttribute : ActionFilterAttribute
    {
        private readonly bool _isDevelopment;
        private readonly Func<IDictionary<string, object>, bool> _validate;


        public CheckModelForNullAttribute(bool isDevelopment)
            : this(arguments => arguments.Any(arg => arg.Value == null), isDevelopment)
        {
        }


        public CheckModelForNullAttribute(Func<IDictionary<string, object>, bool> checkCondition, bool isDevelopment)
        {
            _validate = checkCondition;
            _isDevelopment = isDevelopment;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_validate(context.ActionArguments))
            {
                context.Result = new ObjectResult(OperationResult.Error("The argument cannot be null"));

                // This will return a 400 with an error in json.
                //context.Result = new BadRequestObjectResult(OperationResult.Error("The argument cannot be null"));

            }

            if (context.ActionArguments.Count < context.ActionDescriptor.Parameters.Count)
            {
                if (context.ModelState.IsValid || !_isDevelopment)
                {
                    context.Result = new ObjectResult(OperationResult.Error("Argument parsing error."));
                }
                else
                {
                    context.Result = new ObjectResult(OperationResult.Error("Argument parsing error. " 
                        + string.Join(".", context.ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage))));
                }

                // This will return a 400 with an error in json.
                //context.Result = new BadRequestObjectResult(OperationResult.Error("The argument cannot be null"));

            }

            base.OnActionExecuting(context);
        }

    }

}
