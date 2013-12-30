namespace EdugameCloud.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The splash screen dto.
    /// </summary>
    [DataContract]
    public class SplashScreenDTO
    {
        #region Properties

        /// <summary>
        /// Gets or sets the recent reports.
        /// </summary>
        [DataMember]
        public List<RecentReportDTO> recentReports { get; set; }

        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        [DataMember]
        public List<ReportDTO> reports { get; set; }

        #endregion
    }
}