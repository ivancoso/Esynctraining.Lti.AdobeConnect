using System;
using System.Collections.Generic;

namespace Esynctraining.Mail.Configuration.Json
{
    public sealed class SystemEmailCollection : List<SystemEmail>, ISystemEmailCollection
    {
        public ISystemEmail GetByToken(string emailToken)
        {
           // Check.Argument.IsNotEmpty(emailToken, "emailToken");

            foreach (ISystemEmail systemEmail in this)
            {
                if (systemEmail.Token.Equals(emailToken))
                {
                    return systemEmail;
                }
            }

            throw new ArgumentOutOfRangeException(string.Format("System email(token='{0}') was not found.", emailToken));
        }

    }

}
