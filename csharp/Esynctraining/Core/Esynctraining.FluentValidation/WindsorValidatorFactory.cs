using System;
using System.Linq;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;
using FluentValidation;

namespace Esynctraining.FluentValidation
{
    // TODO: FIX CLASS NAME!!!
    /// <summary>
    /// The windsor validator factory.
    /// </summary>
    public class WindsorValidatorFactory : ValidatorFactoryBase
    {
        private readonly IServiceLocator _container;
        private readonly ILogger _logger;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorValidatorFactory"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public WindsorValidatorFactory(IServiceLocator container, ILogger logger)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The create instance.
        /// </summary>
        /// <param name="validatorType">
        /// The validator type.
        /// </param>
        /// <returns>
        /// The <see cref="IValidator"/>.
        /// </returns>
        public override IValidator CreateInstance(Type validatorType)
        {
            try
            {
                if (validatorType.IsGenericType)
                {
                    var genericArgType = validatorType.GetGenericArguments().FirstOrDefault();
                    if (genericArgType != null && genericArgType.IsClass && genericArgType != typeof(string) && genericArgType.Name != "BaseViewModel")
                    {
                        return this._container.GetInstance(validatorType) as IValidator;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"CreateInstance failed for { validatorType.ToString() }", ex);
                return null;
            }
        }

        #endregion

    }

}
