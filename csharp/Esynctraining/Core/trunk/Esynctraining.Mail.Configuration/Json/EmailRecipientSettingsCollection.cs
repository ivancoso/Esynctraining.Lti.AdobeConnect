using System;
using System.Collections.Generic;

namespace Esynctraining.Mail.Configuration.Json
{
    public sealed class EmailRecipientSettingsCollection : List<EmailRecipientSettings>, IEmailRecipientSettingsCollection
    {
        public IEmailRecipientSettings GetByToken(string emailToken)
        {
           // Check.Argument.IsNotEmpty(emailToken, "emailToken");

            foreach (IEmailRecipientSettings systemEmail in this)
            {
                if (systemEmail.Token.Equals(emailToken))
                {
                    return systemEmail;
                }
            }

            throw new ArgumentOutOfRangeException(string.Format("EmailRecipientSettings (token='{0}') was not found.", emailToken));
        }

    }

}
