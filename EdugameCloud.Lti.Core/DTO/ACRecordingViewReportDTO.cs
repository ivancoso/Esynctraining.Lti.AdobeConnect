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


        public ACRecordingViewReportDTO(RecordingTransactionDTO acSession, int timezoneOffset)
        {
            if (acSession == null)
                throw new ArgumentNullException(nameof(acSession));

            this.scoId = acSession.RecordingScoId;
            this.scoName = acSession.RecordingName;
            this.dateTimeEntered = acSession.DateCreated.ConvertToClientTime(timezoneOffset);
            this.dateTimeLeft = acSession.DateClosed?.ConvertToClientTime(timezoneOffset);
            this.login = acSession.Login;
            this.userName = acSession.UserName;
        }

    }

}
