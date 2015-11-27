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
        private readonly IServiceLocator container;
        private readonly ILogger logger;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorValidatorFactory"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public WindsorValidatorFactory(IServiceLocator container, ILogger logger)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.container = container;
            this.logger = logger;
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
                        return this.container.GetInstance(validatorType) as IValidator;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {


                return null;
            }
        }

        #endregion

    }

}
