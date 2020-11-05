using System;

namespace Esynctraining.AdobeConnect
{
    public interface IChatTranscriptService
    {
        ChatTranscript GetMeetingChatTranscript(string accountId, string meetingScoId, DateTime sessionDateCreated, DateTime sessionDateEnd);

    }

}
