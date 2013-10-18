namespace Esynctraining.Core.Providers
{
    using System;
    using System.Linq;

    using Castle.Windsor;

    using FluentValidation;

    /// <summary>
    /// The windsor validator factory.
    /// </summary>
    public class WindsorValidatorFactory : ValidatorFactoryBase
    {
        #region Fields

        /// <summary>
        /// The container.
        /// </summary>
        private readonly IWindsorContainer container;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorValidatorFactory"/> class.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public WindsorValidatorFactory(IWindsorContainer container)
        {
            this.container = container;
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
                        return this.container.Resolve(validatorType) as IValidator;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}