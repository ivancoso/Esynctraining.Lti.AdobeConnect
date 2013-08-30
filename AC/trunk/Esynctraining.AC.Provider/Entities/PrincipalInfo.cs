namespace Esynctraining.AC.Provider.Entities
{
    using System;

    /// <summary>
    /// PrincipalInfo structure
    /// </summary>
    [Serializable]
    public class PrincipalInfo
    {
        /// <summary>
        /// Gets or sets the principal preferences.
        /// </summary>
        public PrincipalPreferences PrincipalPreferences { get; set; }

        /// <summary>
        /// Gets or sets the principal detail.
        /// </summary>
        public PrincipalDetail PrincipalDetail { get; set; }
    }
}
