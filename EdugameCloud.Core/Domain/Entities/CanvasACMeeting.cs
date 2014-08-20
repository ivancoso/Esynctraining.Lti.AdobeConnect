namespace EdugameCloud.Core.Domain.Entities
{
    using Esynctraining.Core.Domain.Entities;

    /// <summary>
    /// The canvas ac meeting
    /// </summary>
    public class CanvasACMeeting : Entity
    {
        /// <summary>
        /// The context id
        /// </summary>
        public virtual string ContextId { get; set; }
        /// <summary>
        /// The sco id
        /// </summary>
        public virtual string ScoId { get; set; }
    }
}
