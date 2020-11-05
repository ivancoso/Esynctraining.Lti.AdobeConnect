using System;
using System.Configuration;

namespace Esynctraining.Mail.Configuration
{
    internal sealed class SystemEmailElementCollection : ConfigurationElementCollection, ISystemEmailCollection
    {
        #region ISystemEmailCollection Members

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

        #endregion

        protected override ConfigurationElement CreateNewElement()
        {
            return new SystemEmailElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            //Check.Argument.IsNotNull(element, "element");
            return ((SystemEmailElement)element).Token;
        }

    }

}
