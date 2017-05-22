using System.Collections.Generic;

namespace Esynctraining.Mail.Configuration
{
    public interface IEmailRecipientSettings
    {
        string Token { get; }

        string FromToken { get; }

        IEnumerable<string> ToTokens { get; }

        IEnumerable<string> CcTokens { get; }

        IEnumerable<string> BccTokens { get; }

    }

}
