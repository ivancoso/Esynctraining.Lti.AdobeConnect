namespace Esynctraining.Core.Weborb.Business.Helpers
{
    using System.Collections;
    using Esynctraining.Core.Enums;

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
        public object @object { get; set; }

        /// <summary>
        ///     Gets or sets the objects.
        /// </summary>
        public object[] objects { get; set; }

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
}