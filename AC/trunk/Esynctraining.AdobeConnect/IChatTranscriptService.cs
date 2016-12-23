namespace Esynctraining.AdobeConnect
{
    public interface IChatTranscriptService
    {
        ChatTranscript GetMeetingChatTranscript(string meetingScoId, string accountId);

    }

}
