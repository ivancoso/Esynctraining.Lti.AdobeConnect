namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The splash screen DTO.
    /// </summary>
    [DataContract]
    public class SplashScreenDTO
    {
        #region Properties

        /// <summary>
        /// Gets or sets the recent reports.
        /// </summary>
        [DataMember]
        public RecentReportDTO[] recentReports { get; set; }

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        [DataMember]
        public ReportDTO[] reports { get; set; }

        #endregion
    }
}