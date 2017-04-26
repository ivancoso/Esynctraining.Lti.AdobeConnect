using System;
using Esynctraining.Core.Extensions;
using EdugameCloud.Lti.DTO;

namespace EdugameCloud.Lti.Core.DTO
{
    public class ACRecordingViewReportDTO
    {
        public string scoId { get; set; }

        public string scoName { get; set; }

        public DateTime dateTimeEntered { get; set; }

        public DateTime? dateTimeLeft { get; set; }

        public string login { get; set; }

        public string userName { get; set; }

        public string duration
        {
            get
            {
                if (!dateTimeLeft.HasValue)
                    return null;

                return (dateTimeLeft.Value - dateTimeEntered).ToString(@"hh\:mm\:ss");
            }
        }


        public ACRecordingViewReportDTO(RecordingTransactionDTO acSession, TimeZoneInfo timeZone)
        {
            if (acSession == null)
                throw new ArgumentNullException(nameof(acSession));

            this.scoId = acSession.RecordingScoId;
            this.scoName = acSession.RecordingName;
            this.dateTimeEntered = FixACValue(acSession.DateCreated, timeZone).Value;
            this.dateTimeLeft = FixACValue(acSession.DateClosed, timeZone);
            this.login = acSession.Login;
            this.userName = acSession.UserName;
        }

        private static DateTime? FixACValue(DateTime? dt, TimeZoneInfo timeZone)
        {
            if (dt.HasValue)
            {
                return FixACValue(dt.Value, timeZone);
            }
            return null;
        }

        private static DateTime? FixACValue(DateTime dt, TimeZoneInfo timeZone)
        {
            var tmp = dt < dt1951 ? (DateTime?)null : new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);

            if (tmp.HasValue)
            {
                return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(tmp.Value, timeZone), DateTimeKind.Utc);
            }
            return null;
        }

        private static readonly DateTime dt1951 = new DateTime(1951, 1, 1);
    }

}
