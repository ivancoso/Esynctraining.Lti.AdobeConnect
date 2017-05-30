using System.Collections.Generic;

namespace Esynctraining.Mail.Configuration
{
    public interface IEmailRecipientSettings
    {
        string Token { get; }

        string FromToken { get; }

        List<string> ToTokens { get; }

        List<string> CcTokens { get; }

        List<string> BccTokens { get; }

    }

}
