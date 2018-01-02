using System.Linq;
using Esynctraining.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Esynctraining.AspNetCore.Filters
{
    //[AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class CheckModelForNullAttribute : ActionFilterAttribute
    {
        private readonly bool _isDevelopment;
        

        public CheckModelForNullAttribute(bool isDevelopment)
        {
            _isDevelopment = isDevelopment;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Validate(context, out string requiredParameterName))
            {
                if (context.ModelState.IsValid && _isDevelopment)
                {
                    // TRICK: returns name of action method parameter, not from query string
                    // So [FromQuery(Name = "queryStringParameterName")] is not supported.
                    context.Result = new ObjectResult(OperationResult.Error($"Argument parsing error. Parameter '{ requiredParameterName }' is required."));
                }
                else if (context.ModelState.IsValid && !_isDevelopment)
                {
                    context.Result = new ObjectResult(OperationResult.Error("Argument parsing error."));
                }
                else
                {
                    context.Result = new ObjectResult(OperationResult.Error("Argument parsing error. "
                        + string.Join(".", context.ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage))));
                }
            }

            base.OnActionExecuting(context);
        }

        private static bool Validate(ActionExecutingContext context, out string requiredParameterName)
        {
            requiredParameterName = null;
            foreach (var parameter in context.ActionDescriptor.Parameters.Cast<ControllerParameterDescriptor>())
            {
                if (parameter.ParameterInfo.IsOptional)
                    continue;

                if (!context.ActionArguments.TryGetValue(parameter.Name, out object argValue) || (argValue == null))
                {
                    requiredParameterName = parameter.Name;
                    return false;
                }
            }

            return true;
        }

    }

}
