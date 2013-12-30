namespace EdugameCloud.MVC.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using FluentValidation;

    /// <summary>
    /// The base validator.
    /// </summary>
    /// <typeparam name="T">
    /// The view model type
    /// </typeparam>
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        #region Methods

        /// <summary>
        /// The fix date binding exceptions.
        /// </summary>
        /// <param name="modelState">
        /// The model state.
        /// </param>
        protected void FixDateBindingExceptions(ModelState modelState)
        {
            this.FixDateBindingExceptions(modelState, "Date is invalid");
        }

        /// <summary>
        /// The fix date binding exceptions.
        /// </summary>
        /// <param name="modelState">
        /// The model state.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        protected void FixDateBindingExceptions(ModelState modelState, string message)
        {
            if (modelState != null && modelState.Errors.Count > 0
                && modelState.Errors.Any(x => x.Exception == null && x.ErrorMessage.Contains("is invalid")))
            {
                ModelError error =
                    modelState.Errors.FirstOrDefault(x => x.Exception == null && x.ErrorMessage.Contains("is invalid"));
                modelState.Errors.Remove(error);
                modelState.Errors.Add(message);
            }
        }

        /// <summary>
        /// The model state contains model binder exceptions.
        /// </summary>
        /// <param name="modelState">
        /// The model state.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool ModelStateContainsModelBinderExceptions(ModelState modelState)
        {
            if (modelState != null && modelState.Errors != null && modelState.Errors.Count > 0
                &&
                modelState.Errors.Any(x => x.ErrorMessage == string.Empty && x.Exception is InvalidOperationException))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The validate model state.
        /// </summary>
        /// <param name="modelState">
        /// The model state.
        /// </param>
        /// <param name="keys">
        /// The keys.
        /// </param>
        protected void ValidateModelState(ModelStateDictionary modelState, List<string> keys)
        {
            foreach (string key in keys)
            {
                if (this.ModelStateContainsModelBinderExceptions(modelState[key]))
                {
                    modelState.AddModelError(key, "Type is invalid");
                }
            }
        }

        #endregion
    }
}