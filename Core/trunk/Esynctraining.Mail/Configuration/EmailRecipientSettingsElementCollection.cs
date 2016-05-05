using System;
using System.Configuration;

namespace Esynctraining.Mail.Configuration
{
    internal sealed class EmailRecipientSettingsElementCollection : ConfigurationElementCollection, IEmailRecipientSettingsCollection
    {
        #region ISystemEmailCollection Members

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

            throw new ArgumentOutOfRangeException(string.Format("System email(token='{0}') was not found.", emailToken));
        }

        #endregion

        protected override ConfigurationElement CreateNewElement()
        {
            return new EmailRecipientSettingsElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //Check.Argument.IsNotNull(element, "element");
            return ((EmailRecipientSettingsElement)element).Token;
        }

    }

}
