using System;

using Esynctraining.Core.Providers;
using Esynctraining.Core.Extensions;

using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Resources;

namespace Esynctraining.FluentValidation
{
    /// <summary>
    /// The fluent validation extensions.
    /// </summary>
    public static class FluentValidationExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The with error.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="errorKey">
        /// The error key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <typeparam name="T">
        /// Type to look property for
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Type of property to look for
        /// </typeparam>
        /// <returns>
        /// The <see cref="IRuleBuilderOptions{T, TProperty}"/>.
        /// </returns>
        public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, int errorKey, string message)
        {
            return rule.Configure(delegate (PropertyRule config) { config.CurrentValidator.ErrorMessageSource = new ErrorResourceSource { ErrorKey = errorKey, ErrorMessage = message }; });
        }

        /// <summary>
        /// The with error.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="errorKey">
        /// The error key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <typeparam name="T">
        /// Type to look property for
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Type of property to look for
        /// </typeparam>
        /// <returns>
        /// The <see cref="IRuleBuilderOptions{T, TProperty}"/>.
        /// </returns>
        public static IRuleBuilderOptions<T, TProperty> WithDynamicError<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule, int errorKey, Func<string> message)
        {
            return rule.Configure(delegate (PropertyRule config) { config.CurrentValidator.ErrorMessageSource = new ErrorResourceSource { ErrorKey = errorKey, ErrorMessageSource = message }; });
        }

        /// <summary>
        /// The with error.
        /// </summary>
        /// <param name="rule">
        /// The rule.
        /// </param>
        /// <param name="errorKey">
        /// The error key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <typeparam name="T">
        /// Type to look property for
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Type of property to look for
        /// </typeparam>
        /// <returns>
        /// The <see cref="IRuleBuilderOptions{T, TProperty}"/>.
        /// </returns>
        public static IRuleBuilderOptions<T, TProperty> WithDynamicError<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Func<string> message)
        {
            return rule.Configure(delegate (PropertyRule config) { config.CurrentValidator.ErrorMessageSource = new ErrorResourceSource { ErrorMessageSource = message }; });
        }

        #endregion

        /// <summary>
        /// The localized fluent validation builder.
        /// </summary>
        /// <typeparam name="T">
        /// Type to look property for
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// Type of property to look for
        /// </typeparam>
        public class ErrorFluentValidationBuilder<T, TProperty>
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ErrorFluentValidationBuilder{T,TProperty}"/> class.
            /// </summary>
            /// <param name="rule">
            /// The rule.
            /// </param>
            /// <param name="errorKey">
            /// The error key.
            /// </param>
            /// <param name="errorMessage">
            /// The error Message.
            /// </param>
            public ErrorFluentValidationBuilder(IRuleBuilderOptions<T, TProperty> rule, int errorKey, string errorMessage)
            {
                this.Rule = rule;
                this.ErrorKey = errorKey;
                this.ErrorMessage = errorMessage;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the resource key.
            /// </summary>
            public int ErrorKey { get; set; }

            /// <summary>
            /// Gets or sets the error message.
            /// </summary>
            public string ErrorMessage { get; set; }

            /// <summary>
            /// Gets or sets the rule.
            /// </summary>
            public IRuleBuilderOptions<T, TProperty> Rule { get; set; }

            #endregion
        }
    }

    /// <summary>
    /// The localized resource source.
    /// </summary>
    public class ErrorResourceSource : IStringSource
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public int ErrorKey { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public Func<string> ErrorMessageSource { get; set; }

        /// <summary>
        /// Gets or sets the resource name.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets the resource type.
        /// </summary>
        public Type ResourceType
        {
            get
            {
                return typeof(IResourceProvider);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetString()
        {
            var message = this.ErrorMessageSource == null ? this.ErrorMessage : this.ErrorMessageSource.With(x => x.Invoke());
            return this.ErrorKey == default(int) ? message : this.ErrorKey + "#_#" + message;
        }

        #endregion
    }

}
