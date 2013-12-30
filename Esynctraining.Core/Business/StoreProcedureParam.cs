namespace Esynctraining.Core.Business
{
    using NHibernate;

    /// <summary>
    /// The store procedure parameter.
    /// </summary>
    public abstract class StoreProcedureParam
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add parameter.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IQuery"/>.
        /// </returns>
        public abstract IQuery AddParam(IQuery query);

        #endregion
    }

    /// <summary>
    /// The store procedure parameter.
    /// </summary>
    /// <typeparam name="T">
    /// Type of parameter
    /// </typeparam>
    public class StoreProcedureParam<T> : StoreProcedureParam
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreProcedureParam{T}"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public StoreProcedureParam(string name, T value)
        {
            this.Name = name;
            this.Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        protected T Value { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add parameter.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IQuery"/>.
        /// </returns>
        public override IQuery AddParam(IQuery query)
        {
            if (typeof(T) == typeof(double))
            {
                return query.SetDouble(this.Name, double.Parse(this.Value.ToString()));
            }
            return query.SetParameter(this.Name, this.Value);
        }

        #endregion
    }
}