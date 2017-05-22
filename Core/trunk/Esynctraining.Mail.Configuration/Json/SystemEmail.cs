namespace Esynctraining.Mail.Configuration.Json
{
    public sealed class SystemEmail : ISystemEmail
    {
        //[ConfigurationProperty("token", IsRequired = true, IsKey = true)]
        public string Token { get; set; }

        //[ConfigurationProperty("name", IsRequired = true)]
        public string Name { get; set; }

        //[ConfigurationProperty("email", IsRequired = true)]
        //[RegexStringValidator(CommonExpressions.Email)]
        public string Email { get; set; }


        //public MailAddress BuildMailAddress()
        //{
        //    return new MailAddress(Email, Name);
        //}

    }

}
