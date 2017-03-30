namespace EdugameCloud.Core.Converters
{
    using System;
    using System.Collections.Generic;

    public abstract class BaseConverter<T, T2> : BaseConverter
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseConverter{T,T2}" /> class.
        /// </summary>
        protected BaseConverter()
            : base(typeof(T), typeof(T2))
        {
        }

        #endregion

        #region Public Methods and Operators
        
        public abstract T2 Convert(T dto, T2 instance, bool flushUpdates = false);

        #endregion
    }

    public abstract class BaseConverter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConverter"/> class.
        /// </summary>
        /// <param name="binderType">
        /// The binder type.
        /// </param>
        protected BaseConverter(Type binderType)
        {
            this.BinderTypes = new List<Type> { binderType };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseConverter"/> class.
        /// </summary>
        /// <param name="binderTypes">
        /// The binder type.
        /// </param>
        protected BaseConverter(params Type[] binderTypes)
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

        #endregion
    }

}