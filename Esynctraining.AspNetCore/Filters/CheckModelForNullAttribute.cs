using System;
using System.Collections.Generic;
using System.Linq;
using Esynctraining.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esynctraining.AspNetCore.Filters
{
    //[AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckModelForNullAttribute : ActionFilterAttribute
    {
        private readonly Func<IDictionary<string, object>, bool> _validate;


        public CheckModelForNullAttribute()
            : this(arguments => arguments.Any(arg => arg.Value == null))
        { }


        public CheckModelForNullAttribute(Func<IDictionary<string, object>, bool> checkCondition)
        {
            _validate = checkCondition;
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
                context.Result = new ObjectResult(OperationResult.Error("Argument parsing error."));

                // This will return a 400 with an error in json.
                //context.Result = new BadRequestObjectResult(OperationResult.Error("The argument cannot be null"));

            }

            base.OnActionExecuting(context);
        }

    }

}
