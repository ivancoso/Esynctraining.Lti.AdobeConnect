namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The canvas adobe connect credentials
    /// </summary>
    public class CanvasConnectCredentials : Entity
    {
        /// <summary>
        /// The canvas domain
        /// </summary>
        public virtual string CanvasDomain { get; set; }
        /// <summary>
        /// The canvas domain
        /// </summary>
        public virtual string ACDomain { get; set; }
        /// <summary>
        /// The canvas domain
        /// </summary>
        public virtual string ACUsername { get; set; }
        /// <summary>
        /// The canvas domain
        /// </summary>
        public virtual string ACPassword { get; set; }
        /// <summary>
        /// The canvas domain
        /// </summary>
        public virtual string ACScoId { get; set; }
        /// <summary>
        /// The canvas domain
        /// </summary>
        public virtual string CanvasToken { get; set; }
        /// <summary>
        /// The canvas domain
        /// </summary>
        public virtual string ACTemplateScoId { get; set; }
    }
}
