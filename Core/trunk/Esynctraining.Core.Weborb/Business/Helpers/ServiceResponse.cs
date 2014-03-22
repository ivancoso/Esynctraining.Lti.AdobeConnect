namespace Esynctraining.Core.Weborb.Business.Helpers
{
    using System.Collections;
    using Esynctraining.Core.Enums;

    using global::Weborb.V3Types;

    /// <summary>
    /// The service response.
    /// </summary>
    public class ServiceResponse 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResponse"/> class.
        /// </summary>
        public ServiceResponse()
        {
            this.status = Errors.CODE_RESULTTYPE_SUCCESS;
        }

        #region Public Properties

        /// <summary>
        ///     Gets or sets the object.
        /// </summary>
        public Hashtable @object { get; set; }

        /// <summary>
        ///     Gets or sets the objects.
        /// </summary>
        public Hashtable[] objects { get; set; }

        /// <summary>
        ///     Gets or sets total objects count (for paging).
        /// </summary>
        public int totalCount { get; set; }

        /// <summary>
        ///     Gets or sets the error.
        /// </summary>
        public Hashtable error { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        public string status { get; set; }

        #endregion
    }

    /// <summary>
    /// The service response.
    /// </summary>
    public class ServiceResponseV3Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResponse"/> class.
        /// </summary>
        public ServiceResponseV3Message()
        {
            this.status = Errors.CODE_RESULTTYPE_SUCCESS;
        }

        #region Public Properties

        /// <summary>
        ///     Gets or sets the object.
        /// </summary>
        public V3Message @object { get; set; }

        /// <summary>
        ///     Gets or sets the objects.
        /// </summary>
        public V3Message[] objects { get; set; }

        /// <summary>
        ///     Gets or sets total objects count (for paging).
        /// </summary>
        public int totalCount { get; set; }

        /// <summary>
        ///     Gets or sets the error.
        /// </summary>
        public V3Message error { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        public string status { get; set; }

        #endregion
    }
}