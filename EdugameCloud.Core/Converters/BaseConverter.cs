namespace EdugameCloud.Core.Converters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The base model binder.
    /// </summary>
    /// <typeparam name="T">
    /// The type of DTO
    /// </typeparam>
    /// <typeparam name="T2">
    /// The type of instance
    /// </typeparam>
    public abstract class BaseConverter<T, T2> : BaseConverter
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseConverter{T,T2}" /> class.
        /// </summary>
        public BaseConverter()
            : base(typeof(T), typeof(T2))
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="dto">
        /// The dto.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="flushUpdates">
        /// The flush updates.
        /// </param>
        /// <returns>
        /// The <see cref="T2"/>.
        /// </returns>
        public abstract T2 Convert(T dto, T2 instance, bool flushUpdates = false);

        #endregion
    }

    /// <summary>
    ///     The base converter.
    /// </summary>
    public abstract class BaseConverter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConverter"/> class.
        /// </summary>
        /// <param name="binderType">
        /// The binder type.
        /// </param>
        public BaseConverter(Type binderType)
        {
            this.BinderTypes = new List<Type> { binderType };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConverter"/> class.
        /// </summary>
        /// <param name="binderTypes">
        /// The binder type.
        /// </param>
        public BaseConverter(params Type[] binderTypes)
        {
            var types = new List<Type>();
            types.AddRange(binderTypes);
            this.BinderTypes = types;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the binder type.
        /// </summary>
        public IList<Type> BinderTypes { get; set; }

        #endregion
    }
}