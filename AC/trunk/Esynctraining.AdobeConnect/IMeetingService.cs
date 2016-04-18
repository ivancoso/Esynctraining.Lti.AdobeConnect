using System.Collections.Generic;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AdobeConnect
{
    public interface IMeetingService
    {
        IEnumerable<ScoContent> GetUserMeetings();

        IEnumerable<ScoContent> GetSharedMeetings();

    }

}
