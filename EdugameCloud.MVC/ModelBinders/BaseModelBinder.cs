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

    /// <summary>
    /// The model binding extensions.
    /// </summary>
    public static class ModelBindingExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get value by name.
        /// </summary>
        /// <param name="valueProvider">
        /// The value provider.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetValueByName(this IValueProvider valueProvider, string name)
        {
            string firstResult = valueProvider.GetValue(name).With(x => x.AttemptedValue);
            return string.IsNullOrWhiteSpace(firstResult) ? valueProvider.GetValue(Inflector.Capitalize(name)).With(x => x.AttemptedValue) : firstResult;
        }

        /// <summary>
        /// The parse date time.
        /// </summary>
        /// <param name="dateString">
        /// The date string.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime ParseDateTime(this string dateString)
        {
            DateTime dateTime;
            if (!DateTime.TryParse(dateString, out dateTime))
            {
                dateTime = DateTime.MinValue;
            }

            return dateTime;
        }

        /// <summary>
        /// The parse date time.
        /// </summary>
        /// <param name="boolString">
        /// The bool String.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static bool ParseBoolean(this string boolString)
        {
            bool result;
            if (!bool.TryParse(boolString, out result))
            {
                result = false;
            }

            if (boolString == "on")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Parses integer from string.
        /// </summary>
        /// <param name="intString">
        /// The integer string.
        /// </param>
        /// <returns>
        /// The <see cref="int"/> instance.
        /// </returns>
        public static int ParseInt(this string intString)
        {
            int result;
            if (!int.TryParse(intString, out result))
            {
                result = 0;
            }

            return result;
        }

        #endregion
    }
}