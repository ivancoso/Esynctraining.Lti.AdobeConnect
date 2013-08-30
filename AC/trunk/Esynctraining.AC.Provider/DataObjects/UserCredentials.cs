namespace Esynctraining.AC.Provider.DataObjects
{
    public class UserCredentials
    {
        public UserCredentials()
        {
        }

        public UserCredentials(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get; set; }

        public string Password { get; set; }
    }
}
