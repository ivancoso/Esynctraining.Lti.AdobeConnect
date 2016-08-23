using System;

namespace Esynctraining.AC.Provider.DataObjects
{
    public class UserCredentials
    {
        public string Login { get; private set; }

        public string Password { get; private set; }

        public string AccountId { get; private set; }
        
        public UserCredentials(string login, string password, string accountId = null)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Non-empty value expected", nameof(login));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Non-empty value expected", nameof(password));

            Login = login;
            Password = password;
            AccountId = accountId;
        }

    }

}
