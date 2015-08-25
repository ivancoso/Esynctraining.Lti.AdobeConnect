using System;
using System.Collections.Generic;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.API.BrainHoney
{
    public interface IBrainHoneyScheduling
    {
        string CheckForBrainHoneySignals(IEnumerable<LmsCompany> brainHoneyCompanies, DateTime lastScheduledRunDate, int meetingId);

    }

}
