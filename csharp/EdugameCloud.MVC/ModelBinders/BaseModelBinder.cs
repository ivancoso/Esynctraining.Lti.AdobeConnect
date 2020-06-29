namespace EdugameCloud.MVC.ModelBinders
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    /// <summary>
    /// The base model binder.
    /// </summary>
    public abstract class BaseModelBinder : IModelBinder
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModelBinder"/> class.
        /// </summary>
        /// <param name="binderType">
        /// The binder type.
        /// </param>
        protected BaseModelBinder(Type binderType)
        {
            this.BinderTypes = new List<Type>() { binderType };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModelBinder"/> class.
        /// </summary>
        /// <param name="binderTypes">
        /// The binder type.
        /// </param>
        protected BaseModelBinder(params Type[] binderTypes)
        {
            var types = new List<Type>();
            types.AddRange(binderTypes);
            this.BinderTypes = types;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the binder type.
        /// </summary>
        public IList<Type> BinderTypes { get; set; }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            var flag = false;
            if (!string.IsNullOrEmpty(bindingContext.ModelName)
                && !bindingContext.ValueProvider.ContainsPrefix(bindingContext.ModelName))
            {
                if (!bindingContext.FallbackToEmptyPrefix)
                {
                    return null;
                }

                bindingContext = new ModelBindingContext
                {
                    ModelMetadata = bindingContext.ModelMetadata,
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                flag = true;
            }

            if (!flag)
            {
                return this.Bind(controllerContext, bindingContext);
            }

            return null;
        }

        public abstract object Bind(ControllerContext controllerContext, ModelBindingContext bindingContext);

        #endregion
    }

    /// <summary>
    /// The base model binder.
    /// </summary>
    /// <typeparam name="T">
    /// The type of binder
    /// </typeparam>
    public abstract class BaseModelBinder<T> : BaseModelBinder
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModelBinder{T}"/> class.
        /// </summary>
        protected BaseModelBinder()
            : base(typeof(T))
        {
        }

        protected BaseModelBinder(params Type[] binderTypes)
            : base(binderTypes)
        {
        }

        #endregion
    }

    /// <summary>
    /// The base model binder.
    /// </summary>
    /// <typeparam name="T">
    /// The type of binder
    /// </typeparam>
    public abstract class BaseModelBinder<T,V> : BaseModelBinder<T>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModelBinder{T}"/> class.
        /// </summary>
        protected BaseModelBinder()
            : base(typeof(T), typeof(V))
        {
        }

        #endregion
    }

}