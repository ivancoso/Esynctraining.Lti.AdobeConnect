namespace EdugameCloud.Lti.AgilixBuzz
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The parsed signal group.
    /// </summary>
    internal sealed class ParsedSignalGroup
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the main signal.
        /// </summary>
        public Signal RepresentativeSignal { get; set; }

        /// <summary>
        /// Gets the latest signal id.
        /// </summary>
        public long LatestSignalId
        {
            get
            {
                if (this.SignalsAssociated != null && this.SignalsAssociated.Any())
                {
                    return this.SignalsAssociated.Max(x => x.SignalId);
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the processing signal type.
        /// </summary>
        public ProcessingSignalType ProcessingSignalType { get; set; }

        /// <summary>
        /// Gets or sets the signals associated.
        /// </summary>
        public List<Signal> SignalsAssociated { get; set; }

        #endregion
    }
}