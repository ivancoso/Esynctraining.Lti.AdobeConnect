namespace Esynctraining.AC.Provider.DataObjects
{
    public class Account
    {
        public Account(string userName, string password)
        {
            Username = userName;
            Password = password;
        }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
