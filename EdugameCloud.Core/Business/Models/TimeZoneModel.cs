namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    /// <summary>
    ///     The TimeZone model.
    /// </summary>
    public class TimeZoneModel : BaseModel<TimeZone, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public TimeZoneModel(IRepository<TimeZone, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}