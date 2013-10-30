namespace Esynctraining.AC.Provider.Exceptions
{
    using System;

    using Esynctraining.AC.Provider.DataObjects.Results;

    /// <summary>
    /// The ac status exception.
    /// </summary>
    public class ACStatusException : Exception
    {
        public ACStatusException(ResultBase result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public ResultBase Result { get; set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public override string Message
        {
            get
            {
                return this.Result == null ? base.Message : this.Result.Status.InnerXml;
            }
        }
    }
}