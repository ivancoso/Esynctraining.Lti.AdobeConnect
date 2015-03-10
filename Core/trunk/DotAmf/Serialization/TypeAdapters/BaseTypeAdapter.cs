using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotAmf.Serialization.TypeAdapters
{
    /// <summary>
    ///     The base model binder.
    /// </summary>
    public abstract class BaseTypeAdapter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeAdapter"/> class.
        /// </summary>
        /// <param name="binderType">
        /// The binder type.
        /// </param>
        public BaseTypeAdapter(RuntimeTypeHandle binderType)
        {
            this.BinderTypes = new List<RuntimeTypeHandle> { binderType };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeAdapter"/> class.
        /// </summary>
        /// <param name="binderTypes">
        /// The binder type.
        /// </param>
        public BaseTypeAdapter(params RuntimeTypeHandle[] binderTypes)
        {
            var types = new List<RuntimeTypeHandle>();
            types.AddRange(binderTypes);
            this.BinderTypes = types;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the binder type.
        /// </summary>
        public ICollection<RuntimeTypeHandle> BinderTypes { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The bind.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public abstract object Adapt(RuntimeTypeHandle type, object value);

        #endregion
    }

    /// <summary>
    /// The base model binder.
    /// </summary>
    /// <typeparam name="T">
    /// The type of binder
    /// </typeparam>
    public abstract class BaseTypeAdapter<T> : BaseTypeAdapter
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseTypeAdapter{T}" /> class.
        /// </summary>
        public BaseTypeAdapter()
            : base(typeof(T).TypeHandle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeAdapter{T}"/> class.
        /// </summary>
        /// <param name="binderTypes">
        /// The binder types.
        /// </param>
        protected BaseTypeAdapter(params RuntimeTypeHandle[] binderTypes)
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
    /// <typeparam name="T1">
    /// </typeparam>
    public abstract class BaseTypeAdapter<T, T1> : BaseTypeAdapter<T1>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeAdapter{T,T1}"/> class. 
        /// </summary>
        public BaseTypeAdapter() : base(typeof(T).TypeHandle, typeof(T1).TypeHandle)
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
    /// <typeparam name="T1">
    /// </typeparam>
    public abstract class BaseTypeAdapter<T, T1, T2> : BaseTypeAdapter<T1>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeAdapter{T,T1,T2}"/> class. 
        /// Initializes a new instance of the <see cref="BaseTypeAdapter{T,T1}"/> class. 
        /// </summary>
        public BaseTypeAdapter()
            : base(typeof(T).TypeHandle, typeof(T1).TypeHandle, typeof(T2).TypeHandle)
        {
        }

        #endregion

    }

}
